using System;
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
    sync = true;
  }

  void Start() {
    
  }

  bool firstFrame = true;
  public bool sync;
  void Update() {
    sync = firstFrame || Input.GetMouseButton(0);
    Sync();

    story.Update();
    player.Update();


    wallTex.Pixelgon(wallPos, wallScale, wallScale.x);


    Transform cam = Camera.main.transform;
    cam.rotation = Quaternion.Euler(viewAngle, 0, 0);
    cam.position = player.pos + new Vector3(0,0,0) + cam.rotation * new Vector3(0, 0, -24);
    player.Render();

    firstFrame = false;
  }

  [Header("New Design Vars")]
  [ShowOnly] public float pixelsPerMeter = 24;
  [ShowOnly] public float viewAngle = 28;
  [ShowOnly] public float test = 0;
  void Sync() {
    pixelsPerMeter = pixelsPerMeter.Sync("Mono.pixelsPerMeter");
    viewAngle      = viewAngle.Sync("Mono.viewAngle");
    test           = test.Sync("Mono.test");
  }
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
    Sync();
    Vector3 input = new Vector3(
      Input.GetAxisRaw("Horizontal"),
      0,
      Input.GetAxisRaw("Vertical")
    ).normalized;


    pos += input * speed * Time.deltaTime;
  }


  [ShowOnly] public string name = "PLAYER";
  [ShowOnly] public float speed = 3.333f;
  void Sync() {
    name  = name.Sync("Player.name");
    speed = speed.Sync("Player.speed");
  }



  public void Render() {
    
    // scale based on texture size
    texture.Pixelgon(pos, Vector3.one);
  }
  
  public Texture texture;
}


public static class Tools {


  static Matrix4x4 matrix = new Matrix4x4();
  static MaterialPropertyBlock props = new MaterialPropertyBlock();
  public static void Pixelgon(this Texture tex, Vector3 pos, Vector3 scale, float tileX = 1) {
    float ppm = Mono.Inst.pixelsPerMeter;

    scale.Scale(new Vector3(tex.width / ppm, tex.height / ppm, 1));
    matrix.SetTRS(
      pos + Vector3.up * tex.height / ppm / 2,
      Quaternion.identity, 
      scale
    );
    props.SetTexture("_MainTex", tex);
    props.SetFloat("_TileX", tileX);
    Graphics.DrawMesh(Mono.Inst.quad, matrix, Mono.Inst.mat, 0, Camera.main, 0, props, false, false, false);
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