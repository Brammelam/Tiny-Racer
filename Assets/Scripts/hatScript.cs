using System.Collections;
using UnityEngine;

public class hatScript : MonoBehaviour
{
    public selectedCar sc;

    public int previousHat;
    public Outline outline;
    public int _index;
    public bool glowing = false;

    public void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void Update()
    {
        //IOS inputs
        if ((Input.touchCount > 0) && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (raycastHit.collider.name == "hatButton")
                {
                    // Outline on clicked hat
                    if (raycastHit.collider.gameObject.tag == this.gameObject.tag)
                    {
                        this.outline.enabled = true;
                        Color d = Color.white;
                        d.a = 0.6f;
                        this.outline.OutlineColor = d;
                        StartCoroutine(Fade());
                    }

                    // Update the hatindex
                    _index = sc.hatIndex;

                    // Logic for enabling/disabling hats

                    // index 0
                    if (this.name == "tophat")
                    {
                        if (_index != 0)
                        {
                            sc.removeNewNode(_index);
                            _index = 0;
                            sc.addNewNode(_index);
                        }
                        else
                        {
                            sc.removeNewNode(_index);
                        }
                    }

                    // index 1
                    else if (this.name == "crown")
                    {
                        if (_index != 1)
                        {
                            sc.removeNewNode(_index);
                            _index = 1;
                            sc.addNewNode(_index);
                        }
                        else
                        {
                            sc.removeNewNode(_index);
                        }
                    }

                    // index 2
                    else if (this.name == "party")
                    {
                        if (_index != 2)
                        {
                            sc.removeNewNode(_index);
                            _index = 2;
                            sc.addNewNode(_index);
                        }
                        else
                        {
                            sc.removeNewNode(_index);
                        }
                    }
                }
            }
        }
    }

    public void FixedUpdate()
    {
        if (sc == null)
        {
            sc = GameObject.FindGameObjectWithTag("sc").GetComponent<selectedCar>();
        }
    }

    private IEnumerator Fade()
    {
        Color c = outline.OutlineColor;
        for (float alpha = .3f; alpha >= 0; alpha -= 0.01f)
        {
            c.a = alpha;
            outline.OutlineColor = c;
            yield return null;
        }
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Outline on clicked hat
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider.gameObject.tag == this.gameObject.tag)
                {
                    this.outline.enabled = true;
                    Color d = Color.white;
                    d.a = 0.6f;
                    this.outline.OutlineColor = d;
                    StartCoroutine(Fade());
                }
            }

            // Update the hatindex
            _index = sc.hatIndex;

            // Logic for enabling/disabling hats

            // index 0
            if (this.name == "tophat")
            {
                if (_index != 0)
                {
                    sc.removeNewNode(_index);
                    _index = 0;
                    sc.addNewNode(_index);
                }
                else
                {
                    sc.removeNewNode(_index);
                }
            }

            // index 1
            else if (this.name == "crown")
            {
                if (_index != 1)
                {
                    sc.removeNewNode(_index);
                    _index = 1;
                    sc.addNewNode(_index);
                }
                else
                {
                    sc.removeNewNode(_index);
                }
            }

            // index 2
            else if (this.name == "party")
            {
                if (_index != 2)
                {
                    sc.removeNewNode(_index);
                    _index = 2;
                    sc.addNewNode(_index);
                }
                else
                {
                    sc.removeNewNode(_index);
                }
            }
        }
    }
}