using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Assets内のC#ファイルをtxtデータに出力するUnityエディターツール
/// GitHub連携機能（push含む）を追加
/// CoinTossGame プロジェクト対応版
/// </summary>
public class ScriptExportTool : EditorWindow
{
    #region Private Fields

    [SerializeField] private bool debugMode = true;
    [SerializeField] private string outputDirectory = "C:/Users/wakam/Desktop/アイデア/「知らないおじぃちゃんが住みついて」（仮）/バックアップ/git";
    [SerializeField] private bool includeSubfolders = true;
    [SerializeField] private bool addFileExtensionToName = true;
    [SerializeField] private bool createSubfolderStructure = true;
    [SerializeField] private Vector2 scrollPosition;
    [SerializeField] private Vector2 gitScrollPosition;

    // Assets全体をスキャン対象に変更
    private const string SCRIPT_FOLDER_PATH = "Assets";
    private const string OUTPUT_EXTENSION = ".txt";
    private const int MAX_FILE_DISPLAY_COUNT = 50;

    private List<string> foundScriptPaths = new List<string>();
    private List<string> latestGitFiles = new List<string>();
    private int totalFoundFiles = 0;
    private bool isScanning = false;
    private bool isGitScanning = false;
    private bool isGitPushing = false;
    private string lastGitCommitHash = "";
    private string gitPushLog = "";

    // Git設定
    [SerializeField] private string gitRemoteName = "origin";
    [SerializeField] private string gitBranchName = "main";
    [SerializeField] private string gitCommitMessage = "Update scripts";

    private enum TabType { AllFiles, GitLatest, GitPush }

    // デフォルトはGitPushタブを表示
    private TabType currentTab = TabType.GitPush;

    // プロジェクトルートパス（Assetsの一つ上）
    private string ProjectRootPath => Application.dataPath.Replace("/Assets", "").Replace("\\Assets", "");

    #endregion

    #region Unity Menu

    [MenuItem("Tools/Script Export Tool")]
    public static void ShowWindow()
    {
        ScriptExportTool window = GetWindow<ScriptExportTool>("Script Export Tool");
        window.minSize = new Vector2(620, 750);
        window.position = new Rect(
            (Screen.currentResolution.width - 620) / 2,
            (Screen.currentResolution.height - 750) / 2,
            620, 750
        );
        window.maxSize = new Vector2(1200, 900);
        window.Show();
    }

    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(outputDirectory))
            outputDirectory = Path.Combine(Application.dataPath, "ExportedScripts");

        ScanScriptFiles();
    }

    private void OnGUI()
    {
        DrawHeader();
        DrawSettings();
        DrawTabs();

        switch (currentTab)
        {
            case TabType.AllFiles:
                DrawFileList();
                DrawExportSection();
                break;
            case TabType.GitLatest:
                DrawGitSection();
                break;
            case TabType.GitPush:
                DrawGitPushSection();
                break;
        }
    }

    #endregion

    #region GUI Drawing Methods

    private void DrawHeader()
    {
        EditorGUILayout.Space(10);

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        EditorGUILayout.LabelField("Script Export Tool", titleStyle);
        EditorGUILayout.LabelField($"対象: {SCRIPT_FOLDER_PATH} 内のC#・MDファイル → txt出力 / Git連携", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.LabelField($"プロジェクト: {ProjectRootPath}", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.Space(10);
        DrawSeparator();
    }

    private void DrawTabs()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Toggle(currentTab == TabType.AllFiles, "全ファイル", "Button"))
            currentTab = TabType.AllFiles;

        if (GUILayout.Toggle(currentTab == TabType.GitLatest, "GitHub最新Push確認", "Button"))
            currentTab = TabType.GitLatest;

        if (GUILayout.Toggle(currentTab == TabType.GitPush, "GitにPush", "Button"))
            currentTab = TabType.GitPush;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);
        DrawSeparator();
    }

    // ─────────────────────────────────────────
    // タブ①：GitにPushするセクション（新規追加）
    // ─────────────────────────────────────────
    private void DrawGitPushSection()
    {
        EditorGUILayout.LabelField("Git Push 設定", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // Push設定入力
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("リモート名:", GUILayout.Width(100));
        gitRemoteName = EditorGUILayout.TextField(gitRemoteName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ブランチ名:", GUILayout.Width(100));
        gitBranchName = EditorGUILayout.TextField(gitBranchName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("コミットメッセージ:", GUILayout.Width(120));
        gitCommitMessage = EditorGUILayout.TextField(gitCommitMessage);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        DrawSeparator();

        EditorGUILayout.LabelField("実行手順（順番にボタンを押してください）", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // ① git add
        GUI.enabled = !isGitPushing;
        if (GUILayout.Button("① git add（変更ファイルをステージング）", GUILayout.Height(30)))
        {
            RunGitCommand("add -A", "ステージング完了（git add -A）");
        }

        EditorGUILayout.Space(3);

        // ② git commit
        GUI.enabled = !isGitPushing;
        if (GUILayout.Button("② git commit（コミット）", GUILayout.Height(30)))
        {
            string safeMessage = gitCommitMessage.Replace("\"", "\\\"");
            RunGitCommand($"commit -m \"{safeMessage}\"", "コミット完了");
        }

        EditorGUILayout.Space(3);

        // ③ git push
        GUI.enabled = !isGitPushing;
        if (GUILayout.Button($"③ git push {gitRemoteName} {gitBranchName}", GUILayout.Height(35)))
        {
            RunGitPush();
        }

        EditorGUILayout.Space(5);

        // まとめて実行ボタン
        GUI.enabled = !isGitPushing;
        GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
        if (GUILayout.Button("▶ add → commit → push を一括実行", GUILayout.Height(40)))
        {
            RunGitAddCommitPush();
        }
        GUI.backgroundColor = Color.white;

        GUI.enabled = true;

        // ログ表示
        if (!string.IsNullOrEmpty(gitPushLog))
        {
            EditorGUILayout.Space(10);
            DrawSeparator();
            EditorGUILayout.LabelField("実行ログ", EditorStyles.boldLabel);

            GUIStyle logStyle = new GUIStyle(EditorStyles.textArea)
            {
                fontSize = 11,
                wordWrap = true
            };

            gitScrollPosition = EditorGUILayout.BeginScrollView(gitScrollPosition, GUILayout.Height(200));
            EditorGUILayout.SelectableLabel(gitPushLog, logStyle, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("ログをクリア", GUILayout.Height(22)))
            {
                gitPushLog = "";
            }
        }

        if (isGitPushing)
        {
            EditorGUILayout.HelpBox("Git処理中...", MessageType.Info);
        }
    }

    // ─────────────────────────────────────────
    // タブ②：最新Push確認セクション（既存機能）
    // ─────────────────────────────────────────
    private void DrawGitSection()
    {
        EditorGUILayout.LabelField("GitHub最新Pushファイル確認", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        if (GUILayout.Button("最新のPushファイルを取得", GUILayout.Height(30)))
        {
            GetLatestGitPushedFiles();
        }

        if (isGitScanning)
        {
            EditorGUILayout.LabelField("Git情報を取得中...", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        if (!string.IsNullOrEmpty(lastGitCommitHash))
        {
            EditorGUILayout.LabelField($"最新コミット: {lastGitCommitHash}", EditorStyles.miniLabel);
        }

        if (latestGitFiles.Count > 0)
        {
            EditorGUILayout.LabelField($"見つかった対象ファイル: {latestGitFiles.Count}個");

            gitScrollPosition = EditorGUILayout.BeginScrollView(gitScrollPosition, GUILayout.Height(200));
            foreach (string file in latestGitFiles)
                EditorGUILayout.LabelField(file, EditorStyles.miniLabel);
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            GUI.enabled = latestGitFiles.Count > 0 && !string.IsNullOrEmpty(outputDirectory);
            if (GUILayout.Button("最新Pushファイルをtxtで出力", GUILayout.Height(30)))
            {
                ExportGitLatestFiles();
            }
            GUI.enabled = true;
        }
        else if (!isGitScanning && lastGitCommitHash != "")
        {
            EditorGUILayout.LabelField("最新のPushに対象ファイルは含まれていません", EditorStyles.centeredGreyMiniLabel);
        }

        EditorGUILayout.Space(5);
        if (GUILayout.Button("出力フォルダーを開く", GUILayout.Height(25)))
            OpenOutputDirectory();
    }

    private void DrawSettings()
    {
        EditorGUILayout.LabelField("設定", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("出力ディレクトリ:", GUILayout.Width(110));
        outputDirectory = EditorGUILayout.TextField(outputDirectory);
        if (GUILayout.Button("選択", GUILayout.Width(50)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("出力ディレクトリを選択", outputDirectory, "");
            if (!string.IsNullOrEmpty(selectedPath))
                outputDirectory = selectedPath;
        }
        EditorGUILayout.EndHorizontal();

        includeSubfolders = EditorGUILayout.Toggle("サブフォルダーを含む", includeSubfolders);
        addFileExtensionToName = EditorGUILayout.Toggle("ファイル名に拡張子を追加", addFileExtensionToName);
        createSubfolderStructure = EditorGUILayout.Toggle("フォルダー構造を再現", createSubfolderStructure);
        debugMode = EditorGUILayout.Toggle("デバッグモード", debugMode);

        if (EditorGUI.EndChangeCheck())
            ScanScriptFiles();

        EditorGUILayout.Space(5);
        DrawSeparator();
    }

    private void DrawFileList()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("発見されたファイル", EditorStyles.boldLabel);
        if (GUILayout.Button("再スキャン", GUILayout.Width(80)))
            ScanScriptFiles();
        EditorGUILayout.EndHorizontal();

        if (isScanning)
        {
            EditorGUILayout.LabelField("スキャン中...", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        if (totalFoundFiles > 0)
        {
            EditorGUILayout.LabelField($"合計: {totalFoundFiles}個のファイルが見つかりました");

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            int displayCount = Mathf.Min(foundScriptPaths.Count, MAX_FILE_DISPLAY_COUNT);
            for (int i = 0; i < displayCount; i++)
            {
                string relativePath = foundScriptPaths[i].Replace(Application.dataPath, "Assets");
                EditorGUILayout.LabelField($"{i + 1}. {relativePath}", EditorStyles.miniLabel);
            }
            if (totalFoundFiles > MAX_FILE_DISPLAY_COUNT)
                EditorGUILayout.LabelField($"... 他{totalFoundFiles - MAX_FILE_DISPLAY_COUNT}個", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("対象ファイルが見つかりませんでした", EditorStyles.centeredGreyMiniLabel);
        }

        EditorGUILayout.Space(5);
        DrawSeparator();
    }

    private void DrawExportSection()
    {
        EditorGUILayout.LabelField("エクスポート", EditorStyles.boldLabel);

        GUI.enabled = totalFoundFiles > 0 && !string.IsNullOrEmpty(outputDirectory);
        if (GUILayout.Button("すべてのスクリプトをtxtで出力", GUILayout.Height(30)))
            ExportAllScripts();
        GUI.enabled = true;

        EditorGUILayout.Space(5);
        if (GUILayout.Button("出力フォルダーを開く", GUILayout.Height(25)))
            OpenOutputDirectory();
    }

    private void DrawSeparator()
    {
        EditorGUILayout.Space(5);
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        EditorGUILayout.Space(5);
    }

    #endregion

    #region Git Push Methods（新規追加）

    /// <summary>
    /// Gitコマンドを汎用実行
    /// </summary>
    private void RunGitCommand(string arguments, string successLabel)
    {
        try
        {
            var result = ExecuteGit(arguments);
            string log = $"[{DateTime.Now:HH:mm:ss}] git {arguments}\n";

            if (!string.IsNullOrEmpty(result.output))
                log += result.output + "\n";
            if (!string.IsNullOrEmpty(result.error))
                log += "[STDERR] " + result.error + "\n";

            gitPushLog = log + gitPushLog;

            if (result.exitCode == 0)
            {
                if (debugMode) Debug.Log($"ScriptExportTool: {successLabel}");
            }
            else
            {
                Debug.LogWarning($"ScriptExportTool: git {arguments} が終了コード {result.exitCode} で終了");
            }

            Repaint();
        }
        catch (Exception e)
        {
            gitPushLog = $"[エラー] {e.Message}\n" + gitPushLog;
            Debug.LogError($"ScriptExportTool: Gitコマンドエラー - {e.Message}");
        }
    }

    /// <summary>
    /// スキャンされたファイルを強制的に git add する
    /// .gitignore で除外されているファイル（Project_Overview.md など）を確実に含めるため
    /// </summary>
    private void ForceAddScannedFiles()
    {
        if (foundScriptPaths == null || foundScriptPaths.Count == 0) return;

        string root = ProjectRootPath;
        var relativePaths = new List<string>();
        foreach (var path in foundScriptPaths)
        {
            string rel = path;
            if (path.StartsWith(root))
            {
                rel = path.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            relativePaths.Add($"\"{rel.Replace("\\", "/")}\"");
        }

        // コマンドラインの長さを考慮して分割実行
        for (int i = 0; i < relativePaths.Count; i += 50)
        {
            var batch = string.Join(" ", relativePaths.Skip(i).Take(50));
            ExecuteGit($"add -f {batch}");
        }
    }

    /// <summary>
    /// git push を実行
    /// </summary>
    private void RunGitPush()
    {
        isGitPushing = true;
        try
        {
            RunGitCommand($"push {gitRemoteName} {gitBranchName}", $"Push完了 → {gitRemoteName}/{gitBranchName}");
        }
        finally
        {
            isGitPushing = false;
        }
    }

    /// <summary>
    /// add → commit → push を一括実行
    /// </summary>
    private void RunGitAddCommitPush()
    {
        isGitPushing = true;
        gitPushLog = "";

        try
        {
            // git add -A
            var addResult = ExecuteGit("add -A");
            AppendLog("add -A", addResult);

            if (addResult.exitCode != 0)
            {
                EditorUtility.DisplayDialog("エラー", "git add に失敗しました。ログを確認してください。", "OK");
                return;
            }

            // git commit
            string safeMessage = gitCommitMessage.Replace("\"", "\\\"");
            var commitResult = ExecuteGit($"commit -m \"{safeMessage}\"");
            AppendLog($"commit -m \"{gitCommitMessage}\"", commitResult);

            // コミットなしでもpushは続行（nothing to commitの場合exitCode=1になる）
            bool nothingToCommit = commitResult.output.Contains("nothing to commit") ||
                                   commitResult.error.Contains("nothing to commit");

            if (commitResult.exitCode != 0 && !nothingToCommit)
            {
                EditorUtility.DisplayDialog("エラー", "git commit に失敗しました。ログを確認してください。", "OK");
                return;
            }

            // git push
            var pushResult = ExecuteGit($"push {gitRemoteName} {gitBranchName}");
            AppendLog($"push {gitRemoteName} {gitBranchName}", pushResult);

            if (pushResult.exitCode == 0)
            {
                EditorUtility.DisplayDialog("成功", $"GitHubへのPushが完了しました！\n{gitRemoteName}/{gitBranchName}", "OK");
                if (debugMode) Debug.Log("ScriptExportTool: Push完了");
            }
            else
            {
                EditorUtility.DisplayDialog("エラー", "git push に失敗しました。ログを確認してください。", "OK");
            }
        }
        catch (Exception e)
        {
            gitPushLog = $"[エラー] {e.Message}\n" + gitPushLog;
            Debug.LogError($"ScriptExportTool: 一括Push処理エラー - {e.Message}");
            EditorUtility.DisplayDialog("エラー", $"処理中にエラーが発生しました:\n{e.Message}", "OK");
        }
        finally
        {
            isGitPushing = false;
            Repaint();
        }
    }

    /// <summary>
    /// ログ文字列を追記
    /// </summary>
    private void AppendLog(string arguments, (string output, string error, int exitCode) result)
    {
        string entry = $"[{DateTime.Now:HH:mm:ss}] git {arguments}  (exit:{result.exitCode})\n";
        if (!string.IsNullOrEmpty(result.output))
            entry += result.output.TrimEnd() + "\n";
        if (!string.IsNullOrEmpty(result.error))
            entry += "[STDERR] " + result.error.TrimEnd() + "\n";
        entry += "─────────────────────────\n";
        gitPushLog += entry;
    }

    /// <summary>
    /// Gitコマンドを実行して結果を返す
    /// </summary>
    private (string output, string error, int exitCode) ExecuteGit(string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            // "git" から「絶対パス」に変更
            FileName = @"C:\Program Files\Git\cmd\git.exe",
            Arguments = arguments,
            // ★変更点：Assets の一つ上（プロジェクトルート）を作業ディレクトリに設定
            WorkingDirectory = ProjectRootPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(startInfo))
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return (output, error, process.ExitCode);
        }
    }

    #endregion

    #region Git Latest Check Methods

    private void GetLatestGitPushedFiles()
    {
        isGitScanning = true;
        latestGitFiles.Clear();
        lastGitCommitHash = "";

        try
        {
            var result = ExecuteGit("diff --name-only HEAD~1 HEAD");

            if (!string.IsNullOrEmpty(result.error) && result.exitCode != 0)
            {
                Debug.LogError($"Git エラー: {result.error}");
                EditorUtility.DisplayDialog("エラー", "Gitコマンドの実行に失敗しました。\nGitリポジトリが初期化されているか確認してください。", "OK");
            }
            else
            {
                GetLatestCommitHash();

                string[] files = result.output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string file in files)
                {
                    if (file.EndsWith(".cs") || file.EndsWith(".md"))
                        latestGitFiles.Add(file);
                }

                if (debugMode)
                    Debug.Log($"最新Push: {latestGitFiles.Count}個のファイルを検出");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Git取得エラー: {e.Message}");
            EditorUtility.DisplayDialog("エラー", "Gitコマンドの実行に失敗しました。", "OK");
        }
        finally
        {
            isGitScanning = false;
        }
    }

    private void GetLatestCommitHash()
    {
        try
        {
            var result = ExecuteGit("rev-parse --short HEAD");
            lastGitCommitHash = result.output.Trim();
        }
        catch (Exception e)
        {
            Debug.LogError($"コミットハッシュ取得エラー: {e.Message}");
        }
    }

    private void ExportGitLatestFiles()
    {
        if (latestGitFiles.Count == 0)
        {
            EditorUtility.DisplayDialog("エラー", "出力するファイルがありません", "OK");
            return;
        }

        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        string gitOutputDir = Path.Combine(outputDirectory, $"GitLatest_{DateTime.Now:yyyyMMdd_HHmmss}");
        Directory.CreateDirectory(gitOutputDir);

        int successCount = 0, errorCount = 0;
        EditorUtility.DisplayProgressBar("Git最新ファイル出力中", "処理中...", 0f);

        try
        {
            for (int i = 0; i < latestGitFiles.Count; i++)
            {
                float progress = (float)i / latestGitFiles.Count;
                string fileName = Path.GetFileName(latestGitFiles[i]);
                EditorUtility.DisplayProgressBar("Git最新ファイル出力中", $"処理中: {fileName}", progress);

                // ★変更点：プロジェクトルートからの相対パスで解決
                string fullPath = Path.Combine(ProjectRootPath, latestGitFiles[i].Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(fullPath))
                {
                    if (ExportSingleScriptToDirectory(fullPath, gitOutputDir)) successCount++;
                    else errorCount++;
                }
                else
                {
                    Debug.LogWarning($"ファイルが見つかりません: {fullPath}");
                    errorCount++;
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        string message = $"Git最新ファイル出力完了\n成功: {successCount}個\nエラー: {errorCount}個\n\n出力先:\n{gitOutputDir}";
        EditorUtility.DisplayDialog("出力結果", message, "OK");

        if (debugMode) Debug.Log($"Git最新ファイル出力: 成功{successCount}個、エラー{errorCount}個");
    }

    private bool ExportSingleScriptToDirectory(string scriptPath, string targetDirectory)
    {
        try
        {
            string scriptContent = File.ReadAllText(scriptPath);
            string extension = Path.GetExtension(scriptPath);
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);

            if (addFileExtensionToName) fileName += extension;

            string outputPath = Path.Combine(targetDirectory, fileName + OUTPUT_EXTENSION);
            File.WriteAllText(outputPath, scriptContent);

            if (debugMode) Debug.Log($"出力完了: {outputPath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"ファイル出力エラー: {scriptPath}: {e.Message}");
            return false;
        }
    }

    #endregion

    #region Core Methods

    /// <summary>
    /// ★変更点：Assets全体（Packages除く）をスキャン
    /// </summary>
    private void ScanScriptFiles()
    {
        isScanning = true;
        foundScriptPaths.Clear();
        totalFoundFiles = 0;

        try
        {
            // Application.dataPath = "C:/Users/wakam/CoinTossGame/Assets"
            string fullScriptPath = Application.dataPath;

            if (!Directory.Exists(fullScriptPath))
            {
                if (debugMode) Debug.LogWarning($"ScriptExportTool: フォルダーが見つかりません: {fullScriptPath}");
                return;
            }

            SearchOption searchOption = includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string[] scriptFiles = Directory.GetFiles(fullScriptPath, "*.cs", searchOption);

            foundScriptPaths.AddRange(scriptFiles);

            // Project_Overview.md を追加（Assets直下またはルート）
            string[] overviewPaths = {
                Path.Combine(fullScriptPath, "Project_Overview.md"),
                Path.Combine(ProjectRootPath, "Project_Overview.md")
            };
            foreach (var path in overviewPaths)
            {
                if (File.Exists(path) && !foundScriptPaths.Contains(path))
                    foundScriptPaths.Add(path);
            }

            totalFoundFiles = foundScriptPaths.Count;

            if (debugMode) Debug.Log($"ScriptExportTool: {totalFoundFiles}個のファイルを発見 (対象: {fullScriptPath})");
        }
        catch (Exception e)
        {
            Debug.LogError($"ScriptExportTool: スキャン中にエラー: {e.Message}");
        }
        finally
        {
            isScanning = false;
        }
    }

    private void ExportAllScripts()
    {
        if (foundScriptPaths.Count == 0)
        {
            EditorUtility.DisplayDialog("エラー", "出力するファイルがありません", "OK");
            return;
        }

        if (!Directory.Exists(outputDirectory))
        {
            try { Directory.CreateDirectory(outputDirectory); }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("エラー", $"出力ディレクトリの作成に失敗:\n{e.Message}", "OK");
                return;
            }
        }

        int successCount = 0, errorCount = 0;
        EditorUtility.DisplayProgressBar("スクリプト出力中", "ファイルを処理しています...", 0f);

        try
        {
            for (int i = 0; i < foundScriptPaths.Count; i++)
            {
                string scriptPath = foundScriptPaths[i];
                float progress = (float)i / foundScriptPaths.Count;
                EditorUtility.DisplayProgressBar("スクリプト出力中", $"処理中: {Path.GetFileNameWithoutExtension(scriptPath)}", progress);

                if (ExportSingleScript(scriptPath)) successCount++;
                else errorCount++;
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        string message = $"出力完了\n成功: {successCount}個\nエラー: {errorCount}個";
        EditorUtility.DisplayDialog("出力結果", message, "OK");

        if (debugMode) Debug.Log($"ScriptExportTool: {message.Replace('\n', ' ')}");
    }

    private bool ExportSingleScript(string scriptPath)
    {
        try
        {
            string scriptContent = File.ReadAllText(scriptPath);
            string extension = Path.GetExtension(scriptPath);
            string fileName = Path.GetFileNameWithoutExtension(scriptPath);

            if (addFileExtensionToName) fileName += extension;

            string outputFileName = fileName + OUTPUT_EXTENSION;
            string outputPath;

            if (createSubfolderStructure)
            {
                // Assetsフォルダ外にあるファイル（ルート直下など）の相対パスを解決
                string relativeDir;
                if (scriptPath.StartsWith(Application.dataPath))
                {
                    relativeDir = Path.GetRelativePath(Application.dataPath, Path.GetDirectoryName(scriptPath));
                }
                else
                {
                    // ルート直下の場合は、出力先ルートに置く
                    relativeDir = "";
                }
                
                string outputSubDir = Path.Combine(outputDirectory, relativeDir);
                if (!string.IsNullOrEmpty(outputSubDir) && !Directory.Exists(outputSubDir)) 
                    Directory.CreateDirectory(outputSubDir);
                
                outputPath = Path.Combine(outputSubDir, outputFileName);
            }
            else
            {
                outputPath = Path.Combine(outputDirectory, outputFileName);
            }

            File.WriteAllText(outputPath, scriptContent);
            if (debugMode) Debug.Log($"ScriptExportTool: 出力完了 - {outputPath}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"ScriptExportTool: ファイル出力エラー - {scriptPath}: {e.Message}");
            return false;
        }
    }

    private void OpenOutputDirectory()
    {
        if (Directory.Exists(outputDirectory))
            EditorUtility.RevealInFinder(outputDirectory);
        else
            EditorUtility.DisplayDialog("エラー", "出力ディレクトリが存在しません", "OK");
    }

    #endregion
}