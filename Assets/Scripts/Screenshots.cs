using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using UnityEditor;
using UnityEngine;


public class Screenshots : MonoBehaviour
{
    private static readonly string screenshotFolder = "Assets/Screenshots";
    private static bool firstScreenshotTaken = false;
    private static double nextScreenshotTime;


    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        if (!Directory.Exists(screenshotFolder))
        {
            Directory.CreateDirectory(screenshotFolder);
            Debug.Log($"Screenshot folder created at: {screenshotFolder}");
        }

        nextScreenshotTime = EditorApplication.timeSinceStartup + 30f;

        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (EditorApplication.timeSinceStartup >= nextScreenshotTime)
        {
            TakeEditorScreenshot();

            if (!firstScreenshotTaken)
            {
                firstScreenshotTaken = true;
                nextScreenshotTime = EditorApplication.timeSinceStartup + 10f * 60f;
            }
            else
            {
                nextScreenshotTime += 10f * 60f;
            }
        }
    }

    private static void TakeEditorScreenshot()
    {
        // Erstelle den Dateinamen und Pfad
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filename = Path.Combine(Application.dataPath, "Screenshots", $"EditorScreenshot_{timestamp}.png");

        // Speichere den Screenshot
        ScreenCapture.CaptureScreenshot(filename);
        Debug.Log($"Editor Screenshot saved at: {filename}");

        // Aktualisiere die Asset-Datenbank, damit der Screenshot im Editor sichtbar ist
        AssetDatabase.Refresh();
    }
}
