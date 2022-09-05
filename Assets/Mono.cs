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
    player.Start();
  }

  bool firstFrame = true;
  public bool sync;
  void Update() {
    sync = firstFrame || Input.GetMouseButton(0);
    Sync();

    story.Update();
    player.Update();


    wallTex.Pixelgon(wallPos, wallScale, false, wallScale.x);


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

  public void Start() {
    pos = new Vector3(0, 0, 0);
    conRing = new Con[] { cons[0], cons[1], cons[2], cons[3] };
    lastCon = cons[0];
  }

  public void Update() {
    Sync();

    foreach (Con con in cons) {
      if (Input.GetKeyDown(con.key)) {
        conIndex++;
        if (conIndex >= conRing.Length) {
          conIndex = 0;
        }

        lastCon = con;
        frame = 0.5f;
        lastFrame = 0;
      }
    }

    if (Input.GetKey(lastCon.key)) {
      frame += speed * Time.deltaTime;
      if (frame >= 4f) {
        frame = 0;
      }

      if (lastFrame != (int)frame){
        pos += lastCon.dir * step;
        lastFrame = (int)frame;
      }
    }
    else {
      frame = 0;
      lastFrame = 0;
    }
  }
  float frame = 0;
  int lastFrame = 0;

  [ShowOnly] public string name = "PLAYER";
  [ShowOnly] public float step = 0.5f;
  [ShowOnly] public float speed = 4f;
  void Sync() {
    name  = name.Sync("Player.name");
    step = step.Sync("Player.step");
    speed = speed.Sync("Player.speed");
  }

  class Con {
    public KeyCode key;
    public float side;
    public Vector3 dir;
  }

  Con[] cons = {
    new Con { key = KeyCode.UpArrow,    side = 0.0f,  dir = new Vector3( 0, 0,  1) },
    new Con { key = KeyCode.DownArrow,  side = 0.75f, dir = new Vector3( 0, 0, -1) },
    new Con { key = KeyCode.LeftArrow,  side = 0.25f, dir = new Vector3(-1, 0,  0) },
    new Con { key = KeyCode.RightArrow, side = 0.5f,  dir = new Vector3( 1, 0,  0) },
  };
  Con[] conRing = new Con[4];
  int conIndex = 0;
  Con lastCon {
    get { 
      if (!Input.GetKey(conRing[conIndex].key)) {
        for (int i = 0; i < conRing.Length; i++) {
          Con con = conRing[i];
          if (Input.GetKey(con.key)) {
            conIndex = i;
            return con;
          }
        }
      }

      return conRing[conIndex]; 
    }
    set { conRing[conIndex] = value; }
  }

  // sync vars from file OnEnable
  // runtime editable *like normal
  // sync vars  to  file OnDisable
  public void Render() {
    // scale based on texture size
    // uv coords to render sprite frames
    texture.Pixelgon(pos, Vector3.one, true, 0.25f, 0.25f, (int)frame / 4f, lastCon.side);
  }
  
  public Texture texture;
}


public static class Tools {
  static Matrix4x4 matrix = new Matrix4x4();
  static MaterialPropertyBlock props = new MaterialPropertyBlock();
  public static void Pixelgon(
    this Texture tex, 
    Vector3 pos, Vector3 scale, 
    bool autoScale = true,
    float tileX = 1, float tileY = 1,
    float offsetX = 0, float offsetY = 0
    ) {

    float ppm = Mono.Inst.pixelsPerMeter;

    scale.Scale(new Vector3(
      tex.width * (autoScale ? tileX : 1) / ppm, 
      tex.height * (autoScale ? tileY : 1) / ppm, 
      1
    ));
    matrix.SetTRS(
      pos + Vector3.up * tex.height * tileY / ppm / 2,
      Quaternion.identity, 
      scale
    );
    props.SetTexture("_MainTex", tex);
    props.SetFloat("_TileX", tileX);
    props.SetFloat("_TileY", tileY);
    props.SetFloat("_OffsetX", offsetX);
    props.SetFloat("_OffsetY", offsetY);
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