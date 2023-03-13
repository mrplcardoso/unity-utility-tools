using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Classe de objeto que cria e carrega 
/// barra baseada em pontução com preenchimento.
/// </summary>
public class BarHolder : MonoBehaviour
{
	[Header("Lists of strucure's UI Image objetcs:")]
	public List<Image> backGrounds = new List<Image>();
	public List<Image> frontGrounds = new List<Image>();

	[Header("UI Image prefabs:")]
	public Image barBackgroundPrefab;
	public Image barFrontPrefab;
	public Image barFillPrefab;

	[Header("Fill UI Image objetc:")]
	[SerializeField]
	//Objeto único por barra
	private Image fill;

	/// <summary>
	/// Retorna a largura da barra varrendo os "tiles" de barra existentes.
	/// </summary>
	public Vector2 barSize
	{
		get
		{
			Vector2 v = -Vector2.one;
			if (baseFillBackFace && backGrounds.Count > 0)
			{
				v = new Vector2(backGrounds[0].rectTransform.rect.width * backGrounds.Count,
																	backGrounds[0].rectTransform.rect.height);
			}
			else if (!baseFillBackFace && frontGrounds.Count > 0)
			{
				v = new Vector2(frontGrounds[0].rectTransform.rect.width * frontGrounds.Count,
																	frontGrounds[0].rectTransform.rect.height);
			}
			return v;
		}
	}
	/// <summary>
	/// Retorna o tamanho equivalente a um ponto do crescimento do preenchimento.
	/// </summary>
	public float rectTileWidth { get; private set; }
	/// <summary>
	/// Retorna o espaço de deslocamento do preenchimento quando ele cresce, 
	/// por conta da ancoragem central.
	/// </summary>
	public float resizeOffset { get; private set; }
	public int currentPoints { get; private set; }
	
	[Header("Point's options:")]
	[Tooltip("Offset for fill object to not grow more than bar's width")]
	public float limitOffset;
	[Tooltip("Maximum point the bar can have. More points means more slow fill object grow per point")]
	public int maxPoints;
	[Tooltip("Initial bar point and initial fill object's width")]
	public int initialPoints;
	/// <summary>
	/// Indica animação de crescimento do preenchimento acontecendo.
	/// Impede que pontuação seja modificada durante uma animação.
	/// </summary>
	private bool inAnimation;
	[Tooltip("Set background or frontground structure as base for fill object's growth")]
	public bool baseFillBackFace;

	void Start()
	{
		rectTileWidth = barSize.x / maxPoints;
		rectTileWidth -= limitOffset;
		resizeOffset = (rectTileWidth / 2) * fill.rectTransform.localScale.x;
		//Zera o preenchimento pois a barra inicia sem pontos
		fill.rectTransform.sizeDelta = new Vector2(0, fill.rectTransform.sizeDelta.y);
		inAnimation = false;
		//Inicializa a barra com pontuação desejada
		Resize(initialPoints);
	}

	/// <summary>
	/// Aumenta ou diminui preenchimento baseado na nova pontuação e 
	/// reajusta tamanho e posição do preenchimento
	/// </summary>
	/// <param name="growth"></param>
	void Resize(int growth = 1)
	{
		if (growth > 0)
		{ if((currentPoints + growth) > maxPoints) return; }
		if (growth < 0)
		{ if ((currentPoints + growth) < 0) return; }

		currentPoints += growth;
		fill.rectTransform.sizeDelta = new Vector2(
				fill.rectTransform.rect.width + growth * (rectTileWidth), fill.rectTransform.rect.height);
		fill.rectTransform.localPosition = new Vector2(
			fill.rectTransform.localPosition.x + growth * (resizeOffset),
			fill.rectTransform.localPosition.y);
	}

	/// <summary>
	/// Aumenta ou diminui preenchimento baseado na nova pontuação e 
	/// reajusta tamanho e posição do preenchimento, rodando uma animação
	/// suavizando o movimento do preenchimento
	/// </summary>
	/// <param name="growth"></param>
	/// <param name="speed"></param>
	/// <returns></returns>
	IEnumerator AnimatedResize(int growth = 1, float speed = 0.1f)
	{
		if (growth > 0)
		{ if ((currentPoints + growth) > maxPoints) yield break; }
		if (growth < 0)
		{ if ((currentPoints + growth) < 0) yield break; }

		inAnimation = true;
		currentPoints += growth;
		Vector2 newsize = new Vector2(fill.rectTransform.rect.width + growth * (rectTileWidth),
			fill.rectTransform.rect.height);
		Vector2 newpos = new Vector2(fill.rectTransform.localPosition.x + growth * (resizeOffset),
				fill.rectTransform.localPosition.y);
		for (float i = 0; i < 1.1; i+=speed)
		{
			fill.rectTransform.sizeDelta = Vector2.Lerp(fill.rectTransform.sizeDelta,
				newsize, i);
			fill.rectTransform.localPosition = Vector2.Lerp(fill.rectTransform.localPosition,
				newpos, i);
			yield return null;
		}
		inAnimation = false;
	}

	void Update()
	{
		if (!inAnimation)
		{
			if (Input.GetKeyDown(KeyCode.C))
			{ StartCoroutine(AnimatedResize(1)); }
			if (Input.GetKeyDown(KeyCode.V))
			{ StartCoroutine(AnimatedResize(5)); }
			if (Input.GetKeyDown(KeyCode.B))
			{ StartCoroutine(AnimatedResize(maxPoints, 0.01f)); }
			if (Input.GetKeyDown(KeyCode.X))
			{ StartCoroutine(AnimatedResize(-1)); }
			if (Input.GetKeyDown(KeyCode.Z))
			{ StartCoroutine(AnimatedResize(-5, 0.01f)); }
		}
	}

	/// <summary>
	/// Função que cria estrutura de barras com background, frontground e preenchimento.
	/// </summary>
	/// <param name="tileNum"></param>
	/// <returns></returns>
	public virtual bool CreateBarStructure(int tileNum)
	{
		if (tileNum < 1) return false;
		bool created = false;
		if(barBackgroundPrefab != null)
		{
			created = true;
			Image back;
			Vector3 pos;
			for (int i = 0; i < tileNum; ++i)
			{
				pos = new Vector3(transform.localPosition.x + 
																			(i * barBackgroundPrefab.sprite.texture.width),
							transform.localPosition.y);
				back = Instantiate(barBackgroundPrefab, Vector3.zero, Quaternion.identity, this.transform);
				back.rectTransform.localPosition = pos;
				back.rectTransform.sizeDelta = new Vector2(barBackgroundPrefab.sprite.rect.size.x,
					barBackgroundPrefab.sprite.rect.size.y);
				backGrounds.Add(back);
			}
		}
		if (barFillPrefab != null && fill == null)
		{
			created = true;
			Vector3 pos = new Vector3(transform.localPosition.x, transform.localPosition.y);
			fill = Instantiate(barFillPrefab, Vector3.zero, Quaternion.identity, this.transform);
			fill.rectTransform.sizeDelta = new Vector2(barFillPrefab.sprite.rect.size.x,
					barFillPrefab.sprite.rect.size.y);
			fill.rectTransform.localPosition = pos; print(fill);
		}
		if (barFrontPrefab != null)
		{
			created = true;
			Image front;
			Vector3 pos;
			for (int i = 0; i < tileNum; ++i)
			{
				pos = new Vector3(transform.localPosition.x +
															(i * barFrontPrefab.sprite.texture.width),
								transform.localPosition.y);
				front = Instantiate(barFrontPrefab, Vector3.zero, Quaternion.identity, this.transform);
				front.rectTransform.localPosition = pos;
				front.rectTransform.sizeDelta = new Vector2(barFrontPrefab.sprite.rect.size.x, 
					barFrontPrefab.sprite.rect.size.y);
				frontGrounds.Add(front);
			}
		}
		return created;
	}
}