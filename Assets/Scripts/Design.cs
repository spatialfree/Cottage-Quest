using System;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Design {
  // read local design variables from file and automatically update them

  public static float Sync(this float value, string name) {
    if (!Mono.Inst.sync) { return value; }

    string str = Find(name);
    if (str != "") {
      return float.Parse(str.Replace('f', ' ').Trim());
    }
    
    return value + Mathf.Sin(Time.time * 3f);
  }

  public static string Sync(this string value, string name) {
    if (!Mono.Inst.sync) { return value; }

    string str = Find(name);
    if (str != "") {
      return str;
    }
    
    // random char
    return "" + 
      (char)Random.Range(32, 126) + (char)Random.Range(32, 126) + 
      (char)Random.Range(32, 126) + (char)Random.Range(32, 126) + 
      (char)Random.Range(32, 126) + (char)Random.Range(32, 126);
  }

  public static string Find(string name) {
    string classStr = name.Split('.')[0].Trim();
    string varStr = name.Split('.')[1].Trim();
    string path = Application.dataPath + "/Mono.cs";
    if (File.Exists(path)) {
      string currentClass = "";
      string[] lines = File.ReadAllLines(path);
      foreach (string line in lines) {
        if (line.Contains("class")) {
          // word after class
          string[] words = line.Split(' ');
          for (int i = 0; i < words.Length; i++) {
            if (words[i] == "class") {
              currentClass = words[i + 1].Trim();
            }
          }
        }

        if (currentClass == classStr) {
          if (line.Contains("ShowOnly") && line.Contains(varStr)) {
            string[] words = line.Split(' ');
            for (int i = 0; i < words.Length; i++) {
              if (words[i].Trim() == "=") {
                string w = words[i + 1];
                w = w.Replace(';', ' ');
                w = w.Replace('"', ' ');
                return w.Trim();
              }
            }
          }
        }
      }
    }

    return "";
  }
}