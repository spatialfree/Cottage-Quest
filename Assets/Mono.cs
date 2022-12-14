using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mono : MonoBehaviour {
  static public Mono Inst { get; private set; }

  public Vector3 manCam;
  Vector3 manCamSmooth;
  public bool man;
  bool oldMan;
  float manT;

	[Header("Classes")]
	public Story story;
	public TextPanel textPanel;
  public Player player;

  [Header("References")]
  public Mesh quad;
  public Material mat;

  [Header("New References")]
  public Texture2D cottageTex;
  public Texture2D castleTex;

  public Material matCurtain;
  public Vector3 hallCurtain;
  public Vector3 roomCurtain;
  public Vector3 mainCurtain;

  public Vector3 hallPos;
  public Vector3 roomPos;
  public Vector3 mainPos;

  // public Vector3 cottagePos;
  public Vector3 camPos, camRawPos;
  public AnimationCurve camCurve;
  public float camT;


  void Awake() {
    Inst = this;
    sync = true;
  }

  void Start() {
		StartCoroutine(story.Tell());
    player.Start();
  }

  bool firstFrame = true;
  public bool sync;
  void Update() {
    sync = firstFrame || Input.GetMouseButton(0);
    Sync();

		textPanel.Update();
    player.Update();


    manT = Mathf.Clamp01(manT + Time.deltaTime / 9f);
    if (man != oldMan) {
      manT = 0;
      oldMan = man;
    }

    Vector3 toSmooth = Vector3.Lerp(manCamSmooth, manCam, camCurve.Evaluate(manT));
    Vector3 fromSmooth = Vector3.Lerp(manCamSmooth, Vector3.zero, camCurve.Evaluate(manT));
    manCamSmooth = man ? toSmooth : fromSmooth;

    // textPanel.visible = Vector3.Distance(manCamSmooth, manCam) < 0.1f;


    Vector3 thresholdPos = new Vector3(64, 522, 0);

    if (player.pos.x < thresholdPos.x) {
      if (camRawPos != hallPos) {
        camRawPos = hallPos;
        camT = 0;
      }

      matrix.SetTRS(
        roomPos + Vector3.back * 10,
        Quaternion.identity,
        roomCurtain
      );
      Graphics.DrawMesh(Mono.Inst.quad, matrix, Mono.Inst.matCurtain, 0);
    } else {
      if (camRawPos != roomPos) {
        camRawPos = roomPos;
        camT = 0;
      }

      matrix.SetTRS(
        hallPos + Vector3.back * 10,
        Quaternion.identity,
        hallCurtain
      );
      Graphics.DrawMesh(Mono.Inst.quad, matrix, Mono.Inst.matCurtain, 0);
    }

    matrix.SetTRS(
      mainPos + Vector3.back * 10,
      Quaternion.identity,
      mainCurtain
    );
    Graphics.DrawMesh(Mono.Inst.quad, matrix, Mono.Inst.matCurtain, 0);







    cottageTex.Pixelgon(Vector3.forward, Vector3.one);
    castleTex.Pixelgon(Vector3.forward + new Vector3(-1000, 0, 0), Vector3.one);







    Transform cam = Camera.main.transform;
    cam.rotation = Quaternion.identity;
    camT = Mathf.Clamp01(camT + Time.deltaTime / 3f);
    camPos = Vector3.Lerp(camPos, camRawPos, camCurve.Evaluate(camT));

    cam.position = camPos + manCamSmooth + new Vector3(0, 0, -20);

    // pixel perfect (constant size)
    int height = Screen.height;
    Camera.main.orthographicSize = height / 4;


    // // attempt at easy clean gif capture
    // if (Input.GetKey(KeyCode.Space)) {
    //   if (captureIndex < 128) {
    //     if (otherFrame) {
    //       ScreenCapture.CaptureScreenshot("Content/" + captureIndex.ToString("0000") + ".png");
    //       captureIndex++;
    //     }
    //     otherFrame = !otherFrame;
    //   } 
    //   else if (Input.GetKeyDown(KeyCode.Space)) {
    //     captureIndex = 0;
    //   }
    // }

    player.Render();

    firstFrame = false;
  }
  Matrix4x4 matrix = new Matrix4x4();
  int captureIndex = 0;
  bool otherFrame;

  [Header("New Design Vars")]
  [ShowOnly] public float pixelsPerMeter = 1;
  [ShowOnly] public float test = 0;
  void Sync() {
    pixelsPerMeter = pixelsPerMeter.Sync("Mono.pixelsPerMeter");
    test           = test.Sync("Mono.test");
  }
}

[Serializable] public class Story {
	// render characters by GetTexture(name)
	public Character[] characters = new Character[] {
		new Character { 
			name = "William", 
			desc = "Main character of the game controlled by the player. Hes a young farmboy of 17.",
		},
		new Character {
			name = "Mom",
			desc = "Williams mom",
		},
		new Character {
			name = "Lord Tyvus",
			desc = "Main antogonist",
		},
		new Character {
			name = "Counselor",
			desc = "The counselor of Lord Tyvus",
		},
		new Character {
			name = "Princess Ruby",
			desc = "Damsel in Distress",
		},
		new Character {
			name = "Bird",
			desc = "Princess Rubys pet bird",
		},
		new Character {
			name = "Timmy",
			desc = "Neighbor's young kid",
		},
	};
	int day = 0;
	public bool next = false;
	public IEnumerator Tell() {
		Mono mono = Mono.Inst;

		bool telling = true;
		while (telling) {
			switch (day) {
				case 0:
					mono.textPanel.Display("day 0", "(dream) cutscenes are what william dreams about each night.");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 0", "player wakes up when the dream sequences is over");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 0", "Inside events are things that happen inside the cottage that the player has to complete before an outside event can happen.");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 0", "Outside events are things that happen right outside the cottage that the player can see from the peek hole of the front door. This must happen before the player can go back to sleep again.");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 0", "About Sleeping- The player isnt forced to go to bed after the outside event. They can do other activities around the house if they want before they go to sleep.");
					yield return new WaitUntil(() => next); next = false;

					day++;
					break;
				case 1:
					mono.textPanel.Display("day 1", "(Dream) Player dreams of approaching doom and terror and is shown lord Tyvus talking to his advisor about summoning pure evil. Tyvus asks his advisor, Hows the girl? We need her to be strong for the ritual.");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 1", "awake");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 1", "mom wakes you up and says you must have been dreaming and that she has to go to the market now that your fathers dead.");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 1", "Player tries front door but its locked.");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 1", "Inside Event: A bat comes from the chimney that you fight.");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 1", "Outside Event: Later your house is shaken by explosions. Outside the door you see people fleeing. Theyre shouting that Lord Tyvuss men have taken the town. ");
					yield return new WaitUntil(() => next); next = false;
					
					day++;
					break;
				case 2:
					mono.textPanel.Display("day 2", "Dream: You dream of a princess, she has a bird in the dream, she asks you to save her");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 2", "awake");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 2", "Inside Event: Knock down spider webs that have appeared. Fight spiders that come out.");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 2", "Outside Event: A very small boy named Timmy comes to the door asking to be let in because hes cold and he lost his whole family. But you say you cant unlock the door and Timmy walks away. Some hero you are.");
					yield return new WaitUntil(() => next); next = false;
					
					day++;
					break;
				case 3:
					mono.textPanel.Display("day 3", "Dream: You dream that Lord Tyvus is having a conversation with his advisor who is saying that theres a young man that destiny shows will defeat him. Lord Tyvus says thats nonsense.");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 3", "awake");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 3", "Inside Event: Theres piles of sand/dust you have to hit and dust bunnies come out that you have to battle");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 3", "Outside Event: Knock on the door, theres a party of adventurers thats formed to defeat Lord Tyvus but you say you cant unlock the door. They say, have you found a key? And you say, I dont have one. They say, damn, thats too bad and leave.");
					yield return new WaitUntil(() => next); next = false;
					
					day++;
					break;
				case 4:
					mono.textPanel.Display("day 4", "Dream: You dream that your dad has left you a sword to go defeat the great evil. (This can happen in the cottage somewhere)");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 4", "awake");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 4", "Inside Event: You wake up to a smell. You have to fight rotten food has come from the garbage pail");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 4", "Outside Event: A traveling merchant comes to the door. May I offer you any wares. He has a lock pick. He says hell sell it to you for 5 gold (but you dont earn/ have any gold). You say you dont have any gold, he says thats too bad and leaves.");
					yield return new WaitUntil(() => next); next = false;
					
					day++;
					break;
				case 5:
					mono.textPanel.Display("day 5", "Dream: You dream that lord Tyvus is performing a ritual. Hes talking to the princess who is strapped to a table saying that the time of summoning is near and that no one will save her from her fate.");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 5", "awake");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 5", "Inside Event: You wake up to a bunch of fireballs in the house, you must have left the stove on! You have to fight the fireballs.");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 5", "Outside Event: The bird that youve seen in the princesss dream is at the front door? It says the final battle is approaching and you must hurry. But you say you cant because the door is locked and the bird flies away. Some hero you are!");
					yield return new WaitUntil(() => next); next = false;
					
					day++;
					break;
				case 6:
					mono.textPanel.Display("day 6", "Dream: You dream of blackness with some flashes and sounds of an epic battle (This will be some type of pixel art montage rather than game maps)");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 6", "awake");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 6", "Outside Event: You wake up to a parade. People are chanting that Lord Tyvus has been defeated. Theyre shouting long live King Timmy!");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 6", "You walk away from the door slowly and look around the house.");
					yield return new WaitUntil(() => next); next = false;
					mono.textPanel.Display("day 6", "Your mom comes in through the front door. Mom: ???huff???huff???my son, Ive made it home. Tyvuss was conquered by the Robinsons boys down the road. He always was such a nice young man. [looks around] Look what youve done to the place, its all a mess! She looks around.  Anyway, Im just glad youre ok. But, I have to ask why didnt you join the others in the fight? Why didnt you come looking for me?  What do you mean, the key is right here where it always is???(But of course its on the lower wall where the player cant see it) Lets get this place cleaned up and ready us some supper.");
					yield return new WaitUntil(() => next); next = false;
					
					day++;
					break;
				default:
					mono.textPanel.Display("null", "beyond the days alloted!");
					
					telling = false;
					break;
			}
		}
	}
}

[Serializable] public class TextPanel {
	public bool visible;
	public TextMeshProUGUI textBox;
	public string from, text;
	public GameObject textArrow;

	public void Display(string from, string text) {
		this.from = from;
		this.text = text;
		index = 0;
		time  = 0;
		visible = true;
	}

	char[] punctuation = { ',', '.', '!', '?' };
	int index = 0;
	float time = 0;
	public void Update()
	{
		GameObject panel = textBox.transform.parent.gameObject;
		if (panel.activeSelf != visible){
			panel.SetActive(visible);
		}

		string text = this.text;
		if (Input.GetKey(KeyCode.Space))
		{
			time -= Time.deltaTime;
			if (time < 0)
			{
				time = text[index] == ' ' ? 0.01f : 0.05f;
				foreach (char p in punctuation)
				{
					if (text[index] == p)
					{
						time += 1f;
						break;
					}
				}

				index = index + 1 >= text.Length ? index : index + 1;
			}
		}

		if (index < text.Length - 1)
		{
			text = text.Insert(index, "<color=#00000000>");
			text = text.Insert(text.Length, "</color>");
		}
		textBox.text = from + ": " + text;

		textArrow.SetActive(!(index < text.Length - 1) && Mathf.Sin(Time.time * 2) > 0);
	}
}

[Serializable] public class Character {
	public string name, desc;
	public Texture texture;
}

[Serializable] public class Player {

  public Vector3 pos, lastPos;

  public void Start() {
    // pos = new Vector3(0, 0, 0);
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

    lastPos = pos;
  }
  float frame = 0;
  int lastFrame = 0;

  [ShowOnly] public string name = "PLAYER";
  [ShowOnly] public float step = 6f;
  [ShowOnly] public float speed = 6f;
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
    new Con { key = KeyCode.UpArrow,    side = 0.0f,  dir = new Vector3( 0,  1,  0) },
    new Con { key = KeyCode.DownArrow,  side = 0.75f, dir = new Vector3( 0, -1,  0) },
    new Con { key = KeyCode.LeftArrow,  side = 0.25f, dir = new Vector3(-1,  0,  0) },
    new Con { key = KeyCode.RightArrow, side = 0.5f,  dir = new Vector3( 1,  0,  0) },
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

[Serializable] public class PID {
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
