



if (Input.touchCount > 0)
{

    if (Input.touchCount > 0 && cam.ScreenToWorldPoint(Input.touches[0].position).x < 0 && deslizandoAzul == false)
        touchAzul = Input.GetTouch(0);

    //if (Input.touchCount > 1 && cam.ScreenToWorldPoint(Input.touches[1].position).x < 0 && deslizandoAzul == false)
      //  touchAzul = Input.GetTouch(1);

    // Handle finger movements based on TouchPhase
    switch (touch.phase)
    {
        //When a touch has first been detected, change the message and record the starting position
        case TouchPhase.Began:


            touchAzul = Input.GetTouch(0);
            multIntA = 1;
            deslizandoAzul = true;
            Debug.Log("Fuera" + touchAzul.phase);
            InicioDeslizarMovil(touchAzul);

            // Record initial touch position.
            message = "Begun ";
            break;

        //Determine if the touch is a moving touch
        case TouchPhase.Moved:


            DeslizandoMovil();
            Debug.Log("Deslizando");
            break;

        case TouchPhase.Ended:

            Debug.Log("FIN TOCAR");
            deslizandoAzul = false;
            FinDeslizarMovil();

            //Si nos movemos a la camara del contrario  no cambiaremos la variable de suelo tocado para pocer volver a lanzar
            if (puntoFinal.x < 0)
                scrBola.SueloTocado(0);


            message = "Ending ";
            break;
    }
}
