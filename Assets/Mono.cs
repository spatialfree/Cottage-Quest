using System;
using System.IO;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mono : MonoBehaviour {

  static public Mono Inst { get; private set; }

  public Story story;
  public Player player;

  [Header("References")]
  public Mesh quad;
  public Material mat;

  [Header("New References")]
  public Texture wallTex;
  public Vector3 wallPos;
  public Vector3 wallScale;

  void Awake() {
    Inst = this;
  }

  void Start() {
    
  }

  public bool refresh = true;
  void Update() {
    pixelsPerMeter = pixelsPerMeter.Sync("mono.pixelsPerMeter");
    viewAngle      = viewAngle.Sync("mono.viewAngle");
    test           = test.Sync("mono.test");

    refresh = Input.GetMouseButtonDown(0); // just for testing? player editable?
    
    
    story.Update();
    player.Update();


    MaterialPropertyBlock props = new MaterialPropertyBlock();
    props.SetTexture("_MainTex", wallTex);
    props.SetFloat("_TileX", wallScale.x);
    matrix.SetTRS(wallPos, Quaternion.identity, wallScale);
    Graphics.DrawMesh(Mono.Inst.quad, matrix, Mono.Inst.mat, 0, Camera.main, 0, props, false, false, false);



    Transform cam = Camera.main.transform;
    cam.rotation = Quaternion.Euler(viewAngle, 0, 0);
    cam.position = player.pos + new Vector3(0,0,0) + cam.rotation * new Vector3(0, 0, -24);
    player.Render();
  }
  Matrix4x4 matrix = new Matrix4x4();

  [Header("New Design Vars")]
  [ShowOnly] public float pixelsPerMeter;
  [ShowOnly] public float viewAngle;
  [ShowOnly] public float test;
}

[Serializable]
public class Story {
  public TextMeshProUGUI textBox;
  public string testString;

  char[] punctuation = { ',', '.', '!', '?' };
  int index = 0;
  float time = 0;
  public void Update() {
    string text = testString;
    if (Input.GetKey(KeyCode.Space)) {
      time -= Time.deltaTime;
      if (time < 0) {
        time = text[index] == ' ' ? 0.01f : 0.05f;
        foreach (char p in punctuation) {
          if (text[index] == p) {
            time += 1f;
            break;
          }
        }

        index = index + 1 >= text.Length ? index : index + 1;
      }
    }

    if (index < text.Length - 1) {
      text = text.Insert(index, "<color=#00000000>");
      text = text.Insert(text.Length, "</color>");
    }
    textBox.text = text;
  }
}

[Serializable]
public class Player {

  public Vector3 pos;

  public void Update() {
    Vector3 input = new Vector3(
      Input.GetAxisRaw("Horizontal"),
      0,
      Input.GetAxisRaw("Vertical")
    ).normalized;


    pos += input * speed * Time.deltaTime;


    name  = name.Sync("player.name");
    speed = speed.Sync("player.speed");
  }

  [ShowOnly] public string name;
  [ShowOnly] public float speed;



  public void Render() {
    
    
    
    // scale based on texture size
    texture.Pixelgon(
      pos + Vector3.up * texture.height / Mono.Inst.pixelsPerMeter / 2, 
      new Vector3(texture.width / Mono.Inst.pixelsPerMeter, texture.height / Mono.Inst.pixelsPerMeter, 1)
    );
  }
  
  public Texture texture;
}


public static class Tools {


  static Matrix4x4 matrix = new Matrix4x4();
  static MaterialPropertyBlock props = new MaterialPropertyBlock();
  public static void Pixelgon(this Texture tex, Vector3 pos, Vector3 scale) {
    matrix.SetTRS(pos, Quaternion.identity, scale);
    props.SetTexture("_MainTex", tex);
    Graphics.DrawMesh(Mono.Inst.quad, matrix, Mono.Inst.mat, 0, Camera.main, 0, props, false, false, false);
  }
}

[Serializable]
public static class Design {
  // read local design variables from file and automatically update them

  public static float Sync(this float value, string name) {
    if (!Mono.Inst.refresh) { return value; }

    string str = Find(name);
    if (str != "") {
      return float.Parse(str);
    }
    
    return Mathf.Sin(Time.time * 3f);
  }

  public static string Sync(this string value, string name) {
    if (!Mono.Inst.refresh) { return value; }

    string str = Find(name);
    if (str != "") {
      return str;
    }
    
    // random char
    return "" + (char)Random.Range(32, 126) + (char)Random.Range(32, 126) + (char)Random.Range(32, 126);
  }

  public static string Find(string name) {
    string classStr = name.Split('.')[0];
    string varStr = name.Split('.')[1];
    string path = Application.dataPath + "/mono.design";
    if (File.Exists(path))
    {
      string currentClass = "";
      string[] lines = File.ReadAllLines(path);
      foreach (string line in lines)
      {
        if (line.Contains(":"))
        {
          currentClass = line.Split(':')[0];
        }

        if (currentClass == classStr)
        {
          string[] parts = line.Split('=');

          if (parts[0].Trim() == varStr)
          {
            return parts[1].Trim();
          }
        }
      }
    }

    return "";
  }
}




[Serializable]
public class PID {
  public float p, i;
  float integral = 0f;
  float value = 0f;
  // float scalar = 1f;

  public PID(float p = 1, float i = 0.1f) {
    this.p = p;
    this.i = i;
  }

  public float Update(float target) {
    float error = value - target;
    integral += error;
    float delta = ((p * error) + (i * integral));
    return value -= delta * Time.deltaTime;
  }
}