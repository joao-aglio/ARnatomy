using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementIndicator : MonoBehaviour
{
    public Object objeto; // Variável publica, para acesso na interface do Unity 

    private ARRaycastManager controleRay; 
    private GameObject visual; // Objeto do indicador
    private Pose lugarPose;
    private bool validaPose = false;

    void Start () // Chama ao iniciar a aplicação
    {
        controleRay = FindObjectOfType<ARRaycastManager>(); // Instancia o objeto do Raycast, para controle
        visual = transform.GetChild(0).gameObject;

        // Esconde o objeto do indicador
        visual.SetActive(false);
    }
    
    void Update () // A cada frame, chama a função
    {
        alteraPosicaoIndicador(); // Chamando função que altera posição do indicador
        alteraPosicaoObjeto(); // Chamando função que altera a posição do objeto
        
        if (validaPose && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) //se o lugar é valido, coloca o objeto
        {
            colocaObjeto(); //Chama a função que coloca o objeto
        }
    }

    private void alteraPosicaoIndicador()
    {
        // Atira um Raycast no centro da tela
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        controleRay.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

        // Se o Raycast acerta uma superfice AR, altera a posicao do objeto do indicador
        if(hits.Count > 0)
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;

            // se o visual do indicador estiver encondido, habilitar
            if(!visual.activeInHierarchy)
                visual.SetActive(true);
        }

    }

    private void alteraPosicaoObjeto()
    {
        var telaCentro = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f)); // Coloca a câmera no centro
        var hits = new List<ARRaycastHit>(); // Tiros do Raycast
        controleRay.Raycast(telaCentro, hits, TrackableType.Planes); // Passa por parametro o centro da tela, os tiros, e o tipo de rastreio

        validaPose = hits.Count > 0; // A varíavel recebe a validação se o número de tiros é maior que 0

        if (validaPose) // Caso seja, altere a posição do objeto
        {
            lugarPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            lugarPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void colocaObjeto()
    {
        Instantiate(objeto, lugarPose.position, lugarPose.rotation); // Passa por parametro o objeto e suas posicoes
    }
}