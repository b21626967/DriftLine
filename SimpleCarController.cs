using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCarController : MonoBehaviour {

    public static SimpleCarController instance;
    public Rigidbody rb;
    public float m_horizontalInput;
    public float m_verticalInput;
    private float m_steeringAngle;

    public WheelCollider frontDriverW, frontPassengerW;
    public WheelCollider rearDriverW, rearPassengerW;
    public Transform frontDriverT, frontPassengerT;
    public Transform rearDriverT, rearPassengerT;
    public float maxSteerAngle = 30;
    public float motorForce;
    public Transform oldWheelcolliderValues;
    int num_of_down = 0;
    int num_of_brakes = 0;
    int num_of_left = 0;
    int num_of_right = 0;
    public bool Acclerate = false;
    bool brake = false;
    public float current_velocity = 0;

    public float max_speed = 100;
    public float max_drift_time = 1f;
    public bool isGallery;
    private void Awake() {

        if (instance == null) {
            instance = this;
        }

        m_verticalInput = 0;
        rb = GetComponent<Rigidbody>();
        //rearPassengerW.brakeTorque = 0.01f;
        //rearDriverW.brakeTorque = 0.01f;
        //rb.centerOfMass = new Vector3(0,-0.5f,0);
        oldWheelcolliderValues = rearPassengerW.transform;

        
    }


    public void GetInput() {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
    }

    private void Steer() {
        if (m_horizontalInput > 0 && current_velocity > 16) {
            steeringEffectRight();

        }else if (m_horizontalInput < 0 && current_velocity > 16) {
            steeringEffectLeft();

        }
        m_steeringAngle = maxSteerAngle * m_horizontalInput;
        frontDriverW.steerAngle = m_steeringAngle;
        frontPassengerW.steerAngle = m_steeringAngle;
    }

    public void Accelerate() {
        if(rearDriverW.transform.position.y > 0.49) {
            
            Invoke("accelarationEffect",0.1f);
                     
            
        }
        rearDriverW.motorTorque = motorForce;
        rearPassengerW.motorTorque = motorForce;
        frontDriverW.motorTorque = motorForce;
        frontPassengerW.motorTorque = motorForce;
    }
    private void Hand_brake() {
        rearDriverW.motorTorque = -1f * motorForce;
        rearPassengerW.motorTorque = -1f * motorForce;
    }
    private void UpdateWheelPoses() {
        UpdateWheelPose(frontDriverW, frontDriverT);
        UpdateWheelPose(frontPassengerW, frontPassengerT);
        UpdateWheelPose(rearDriverW, rearDriverT);
        UpdateWheelPose(rearPassengerW, rearPassengerT);
    }

    private void UpdateWheelPose(WheelCollider _collider, Transform _transform) {
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);
        //_pos.z += 0.1f;
        _transform.position = _pos;
        if (!isGallery) {
            _transform.rotation = _quat;
        }
        
        
    }

    private void FixedUpdate() {
        //GetInput();
        //Accelerate();
        Steer();        
        UpdateWheelPoses();
        current_velocity = rb.velocity.sqrMagnitude;
        if (current_velocity > max_speed) {
            rb.velocity *= 0.9f;
        }

        //Hand_brake();
        

    }
    public void RELEASETheGass() {
        rearDriverW.motorTorque = 0;
        rearPassengerW.motorTorque = 0;
        frontDriverW.motorTorque = 0;
        frontPassengerW.motorTorque = 0;

    }

    public void accelarationEffect() {
        
        rearPassengerW.transform.position += new Vector3(0, 0.01f, 0);
        rearDriverW.transform.position += new Vector3(0, 0.01f, 0);
        num_of_down++;
        Invoke("accelarationEffectRewind", 0.4f);
        Invoke("needmoreacc",0.1f);
       
    }
    public void accelarationEffectRewind() {
        
        rearPassengerW.transform.position -= new Vector3(0, 0.01f, 0);
        rearDriverW.transform.position -= new Vector3(0, 0.01f, 0);
        num_of_down--;
    }

    public void brakingEffect() {
        frontPassengerW.transform.position += new Vector3(0, 0.01f, 0);
        frontDriverW.transform.position += new Vector3(0, 0.01f, 0);
        num_of_brakes++;
        Invoke("brakingEffectRewind", 0.4f);
        Invoke("needmorebrake", 0.1f);
    }

    public void brakingEffectRewind() {
        frontPassengerW.transform.position -= new Vector3(0, 0.01f, 0);
        frontDriverW.transform.position -= new Vector3(0, 0.01f, 0);
        num_of_brakes--;
    }

    public void steeringEffectRight() {
        
        frontDriverW.transform.position += new Vector3(0, 0.0025f, 0);
        //rearDriverW.transform.position += new Vector3(0, 0.0025f, 0);
        num_of_right++;
        Invoke("steeringEffectRightRewind", 0.4f);
        Invoke("needmoreright", 0.1f);       
        
    }
    public void steeringEffectLeft() {
        frontPassengerW.transform.position += new Vector3(0, 0.0025f, 0);
        //rearPassengerW.transform.position += new Vector3(0, 0.0025f, 0);
        num_of_left++;
        Invoke("steeringEffectLeftRewind", 0.4f);
        Invoke("needmoreleft", 0.1f);
    }
        public void steeringEffectLeftRewind() {
        frontPassengerW.transform.position -= new Vector3(0, 0.0025f, 0);
        //rearPassengerW.transform.position -= new Vector3(0, 0.0025f, 0);
        num_of_left--;
    }
    public void steeringEffectRightRewind() {
        frontDriverW.transform.position -= new Vector3(0, 0.0025f, 0);
        //rearDriverW.transform.position -= new Vector3(0, 0.0025f, 0);
        num_of_right--;
        
    }

    public void isAccelareting() {
        Acclerate = true;
    }

    public void isNotAccelareting() {
        Acclerate = false;
    }
    public void needmoreacc() {
        if (Acclerate && num_of_down < 8 && current_velocity < 90) {
            accelarationEffect();
        }
    }

    public void isBraking() {
        brake = true;
    }

    public void isNotBraking() {
        brake = false;
    }
    public void needmorebrake() {
        if (brake && num_of_brakes < 6 && current_velocity > 10) {
           brakingEffect();
        }
    }

    public void needmoreleft() {
        if (m_horizontalInput < 0 && num_of_left < 6) {
            steeringEffectLeft();
        }
    }
    public void needmoreright() {
        if (m_horizontalInput > 0 && num_of_right < 6) {
            steeringEffectRight();
        }
    }

    public int getCarSpeed() {

        return (int)rb.velocity.sqrMagnitude;

    }
    public void steerLeft() {
        m_horizontalInput = -1;

    }

    public void steerRight() {
        m_horizontalInput = 1;
    }

    public void noSteer() {
        m_horizontalInput = 0;

    }
    public void Top_speed_goes_normal() {
        max_speed = 100;
    }


}