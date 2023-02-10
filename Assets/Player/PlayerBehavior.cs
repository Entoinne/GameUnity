using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerBehavior : MonoBehaviour
{
	public Rigidbody m_RigidBody;

	private Node root;

	[SerializeField]
	private Canvas MoveCanvas;

	[SerializeField]
	private Canvas FightCanvas;

	[SerializeField]
	private Canvas AttackCanvas;

	private float epsilonDistance = 0.001f;

	private Stack<Node> chemin;

	public Camera cam;

	[SerializeField] enemyBehavior leftEnemy;
	[SerializeField] enemyBehavior rightEnemy;

	[SerializeField]
	private Camera fightCam1;

	[SerializeField]
	private Camera fightCam2;

	[SerializeField]
	private Camera fightCam3;

	[SerializeField]
	private Camera fightCam4;

	Animator animator;

	[SerializeField]
	private Button BouttonDevant;

	[SerializeField]
	private Button BouttonArriere;

	[SerializeField]
	private Button BouttonDroit;

	[SerializeField]
	private Button BouttonGauche;

	[SerializeField]
	private Text ObjectifUn;

	[SerializeField]
	private Text ObjectifDeux;

	[SerializeField]
	private Text ObjectifTrois;

	private Vector3 CombatZone;

	[SerializeField]
	private float hp = 100;

	private GameObject ButtonAxis;

	private System.Random rand;

	private float winningFight;

	private Boolean isFighting;
	
	private Vector3 sortie;

	//Node class
	public class Node
	{
		public Vector3 val;
		public Node[] child = new Node[5];

		public Node(Vector3 P)
		{
			val = P;
			for (int i = 0; i < 5; i++)
				child[i] = null;
		}
	};

	void Awake()
	{
		isFighting = false;
		winningFight = 0;
		rand = new System.Random();
		chemin = new Stack<Node>();

		root = new Node(m_RigidBody.transform.position);
		root.child[1] = new Node(GameObject.Find("Checkpoint").transform.position);
		root.child[1].child[1] = new Node(GameObject.Find("Checkpoint (1)").transform.position);
		root.child[1].child[2] = new Node(GameObject.Find("Checkpoint (2)").transform.position);
		root.child[1].child[1].child[0] = new Node(
			GameObject.Find("Checkpoint (3)").transform.position
		);
		sortie = root.child[1].child[1].child[0].val;
		CombatZone = GameObject.Find("Checkpoint (2)").transform.position;

		fightCam1.enabled = false;
		fightCam2.enabled = false;
		fightCam3.enabled = false;
		fightCam4.enabled = false;
		AttackCanvas.gameObject.SetActive(false);
		FightCanvas.gameObject.SetActive(false);
		MoveCanvas.gameObject.SetActive(true);
	}

	// Start is called before the first frame update
	void Start()
	{
		ButtonAxis = GameObject.Find("ButtonAxis");
		animator = GetComponentInChildren<Animator>();
		Debug.Log("Game start !");
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.name == "Checkpoint")
		{
			Debug.Log("Checkpoint atteint");
		}
	}

	private IEnumerator MoveCoroutine(Vector3 target)
	{
		float t = 0.0f;
		float elapsed = 0.0f;
		float duration = 1.5f;
		Vector3 start = transform.position;
		var y = Quaternion.LookRotation(target - cam.transform.position);
		yield return StartCoroutine(RotationCoroutine(y));
		while (isActiveAndEnabled && t < 1.0f)
		{
			yield return new WaitForEndOfFrame();
			t = elapsed / duration; // 0 1 [240, 360]
			transform.position = Vector3.Lerp(start, target, t);
			elapsed += Time.deltaTime;
		}
	}

	private IEnumerator AttackMoveCoroutine(Vector3 target)
	{
		float t = 0.0f;
		float elapsed = 0.0f;
		float duration = 1.3f;
		Vector3 start = transform.position;
		while (isActiveAndEnabled && t < 1.0f)
		{
			yield return new WaitForEndOfFrame();
			t = elapsed / duration; // 0 1 [240, 360]
			transform.position = Vector3.Lerp(start, target, t);
			elapsed += Time.deltaTime;
		}
	}

	private IEnumerator RotationCoroutine(Quaternion y)
	{
		float t = 0.0f;
		float elapsed = 0.0f;
		float duration = 0.5f;
		Quaternion start = cam.transform.rotation;
		while (isActiveAndEnabled && t < 1.0f)
		{
			yield return null;
			t = elapsed / duration; // 0 1 [240, 360]
			cam.transform.rotation = Quaternion.Slerp(start, y, t);
			elapsed += Time.deltaTime;
		}
	}

	private IEnumerator FightCoroutine()
	{
		//Tant que les hps du joueurs ou des monstres sont au dessus de 0
		// hp joueur = 100, hp monstre = 25
		Debug.Log("Entrée en combat");
		animator.SetBool("isFighting", true);
		AttackCanvas.gameObject.SetActive(true);
		while (isActiveAndEnabled && hp > 0 && (leftEnemy.hp > 0 || rightEnemy.hp > 0))
		{
			yield return StartCoroutine(FightCameraCoroutine());
		}
		MoveCanvas.gameObject.SetActive(true);
		FightCanvas.gameObject.SetActive(false);
		AttackCanvas.gameObject.SetActive(false);
		winningFight += 1;
		isFighting = false;
		cam.enabled = true;
		fightCam1.enabled = false;
		fightCam2.enabled = false;
		fightCam3.enabled = false;
		fightCam4.enabled = false;
	}

	private IEnumerator AttackCoroutine(enemyBehavior enemyAttacked)
	{
		AttackCanvas.gameObject.SetActive(false);
		enemyAttacked.hp -= rand.Next(3, 13);
		animator.SetBool("isAttacking", true);
		StartCoroutine(AttackMoveCoroutine(new Vector3(-12.379f, 2.08144426f, 4.49552536f)));
		yield return new WaitForSeconds(1.3f);
		enemyAttacked.animator.SetBool("isAttacked", true);
		yield return new WaitForSeconds(0.5f);
		enemyAttacked.animator.SetBool("isAttacked", false);
		animator.SetBool("isAttacking", false);
		StartCoroutine(AttackMoveCoroutine(new Vector3(-9.51907253f, 2.08144426f, 4.49552536f)));
		yield return new WaitForSeconds(2);
		enemyAttacked.animator.SetBool("isAttacking", true);
		yield return new WaitForSeconds(1.3f);
		animator.SetBool("isAttacked", true);
		yield return new WaitForSeconds(0.5f);
		enemyAttacked.animator.SetBool("isAttacking", false);
		animator.SetBool("isAttacked", false);
		hp -= rand.Next(0, 4);
		AttackCanvas.gameObject.SetActive(true);
	}

	private IEnumerator FightCameraCoroutine()
	{
		if (cam.enabled == true)
		{
			cam.enabled = false;
			fightCam1.enabled = true;
		}
		else
		{
			if (fightCam1.enabled == true)
			{
				fightCam1.enabled = false;
				fightCam2.enabled = true;
			}
			else
			{
				if (fightCam2.enabled == true)
				{
					fightCam2.enabled = false;
					fightCam3.enabled = true;
				}
				else
				{
					if (fightCam3.enabled == true)
					{
						fightCam3.enabled = false;
						fightCam4.enabled = true;
					}
					else
					{
						if (fightCam4.enabled == true)
						{
							fightCam4.enabled = false;
							fightCam1.enabled = true;
						}
					}
				}
			}
		}
		yield return new WaitForSeconds(3);
	}

	public void AttaqueGauche()
	{
		StartCoroutine(AttackCoroutine(leftEnemy));
	}

	public void AttaqueDroite()

	{
		StartCoroutine(AttackCoroutine(rightEnemy));
	}

	public void BtnFight()
	{
		StartCoroutine(FightCoroutine());
		isFighting = true;
	}

	public void BtnGauche()
	{
		ButtonAxis.transform.rotation = Quaternion.Euler(0, 0, 90);
		chemin.Push(root);
		root = root.child[0];
		StartCoroutine(MoveCoroutine(root.val));
	}

	public void BtnDroite()
	{
		ButtonAxis.transform.rotation = Quaternion.Euler(0, 0, 90);
		chemin.Push(root);
		root = root.child[2];
		StartCoroutine(MoveCoroutine(root.val));
	}

	public void BtnArriere()
	{
		ButtonAxis.transform.rotation = Quaternion.Euler(0, 0, -180);
		root = chemin.Pop();
		StartCoroutine(MoveCoroutine(root.val));
	}

	public void BtnDevant()
	{
		ButtonAxis.transform.rotation = Quaternion.Euler(0, 0, 90);
		chemin.Push(root);
		root = root.child[1];
		StartCoroutine(MoveCoroutine(root.val));
	}

	// Update is called once per frame
	void Update()
	{
		if (Vector3.Distance(sortie, m_RigidBody.transform.position) < epsilonDistance)
		{
			Application.Quit();
		}
		if (hp < 0)
		{
			MoveCanvas.gameObject.SetActive(false);
			animator.SetBool("isDead", true);
		}
		else
		{
			if (GameObject.Find("Checkpoint (2)") == null)
			{
				ObjectifUn.color = Color.green;
				if (
					GameObject.Find("Checkpoint (1)") == null
					&& GameObject.Find("Checkpoint (3)") == null
				)
				{
					ObjectifTrois.color = Color.green;
				}
			}
			if (winningFight > 0)
			{
				ObjectifDeux.color = Color.green;
			}
			if (Vector3.Distance(m_RigidBody.transform.position, root.val) < epsilonDistance)
			{
				if (Vector3.Distance(m_RigidBody.transform.position, CombatZone) < epsilonDistance)
				{
					if (winningFight == 0)
					{
						if (isFighting == false)
						{
							MoveCanvas.gameObject.SetActive(false);
							FightCanvas.gameObject.SetActive(true);
							AttackCanvas.gameObject.SetActive(false);
						}
						else
						{
							MoveCanvas.gameObject.SetActive(false);
							FightCanvas.gameObject.SetActive(false);
						}
					}
					else
					{
						MoveCanvas.gameObject.SetActive(true);
						FightCanvas.gameObject.SetActive(false);
						AttackCanvas.gameObject.SetActive(false);
					}
				}
				else
				{
					MoveCanvas.gameObject.SetActive(true);
					FightCanvas.gameObject.SetActive(false);
					AttackCanvas.gameObject.SetActive(false);
				}
				if (root.child[1] != null)
				{
					BouttonDevant.interactable = true;
				}
				else
				{
					BouttonDevant.interactable = false;
				}
				if (root.child[2] != null)
				{
					BouttonDroit.interactable = true;
				}
				else
				{
					BouttonDroit.interactable = false;
				}
				if (root.child[0] != null)
				{
					BouttonGauche.interactable = true;
				}
				else
				{
					BouttonGauche.interactable = false;
				}
				if (chemin.Count > 0)
				{
					BouttonArriere.interactable = true;
				}
				else
				{
					BouttonArriere.interactable = false;
				}
				animator.SetBool("isMoving", false);
			}
			else
			{
				MoveCanvas.gameObject.SetActive(false);
				animator.SetBool("isMoving", true);
			}
		}
	}
}
//quaternion modif la rotation seulement de la caméra, euler angle, smoothdamp(vector3)
