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

  void Awake() {
    Inst = this;
  }

  void Start() {
    
  }

  void Update() {
    story.Update();
    player.Update();
    Camera.main.transform.position = player.pos + new Vector3(0, 16, -24);


    player.Render();
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
    Vector3 input = new Vector3(
      Input.GetAxisRaw("Horizontal"),
      0,
      Input.GetAxisRaw("Vertical")
    ).normalized;


    pos += input * speed * Time.deltaTime;
  }

  public string name;
  float speed = 3f;



  public void Render() {
    MaterialPropertyBlock props = new MaterialPropertyBlock();
    props.SetTexture("_MainTex", texture);
    Graphics.DrawMesh(Mono.Inst.quad, pos + Vector3.up * 0.5f, Quaternion.identity, Mono.Inst.mat, 0, Camera.main, 0, props);
  }
  public Texture texture;
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