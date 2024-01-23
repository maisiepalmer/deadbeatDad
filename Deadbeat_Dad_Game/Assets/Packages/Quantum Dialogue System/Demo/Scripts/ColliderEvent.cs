using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Exposes 2D physics 'OnCollisionXXX2D' and `OnTriggerXXX2D' callbacks as events.
///
/// adapted from Melv May's on unity forums
/// https://forum.unity.com/threads/is-it-possible-to-define-physics-messages-like-ontriggerenter-from-different-gameobject.784679/#post-5222876
/// </summary>
public class ColliderEvent : MonoBehaviour
{
    [Serializable]
    public class CollisionEnterEvent : UnityEvent<Collision> { }
 
    [Serializable]
    public class CollisionStayEvent : UnityEvent<Collision> { }
 
    [Serializable]
    public class CollisionExitEvent : UnityEvent<Collision> { }
 
    [Serializable]
    public class TriggerEnterEvent : UnityEvent<Collider> { }
 
    [Serializable]
    public class TriggerStayEvent : UnityEvent<Collider> { }
 
    [Serializable]
    public class TriggerExitEvent : UnityEvent<Collider> { }
 
    [SerializeField]
    private CollisionEnterEvent m_OnCollisionEnter = new CollisionEnterEvent ();
 
    [SerializeField]
    private CollisionStayEvent m_OnCollisionStay = new CollisionStayEvent ();
 
    [SerializeField]
    private CollisionExitEvent m_OnCollisionExit = new CollisionExitEvent ();
 
    [SerializeField]
    private TriggerEnterEvent m_OnTriggerEnter = new TriggerEnterEvent ();
 
    [SerializeField]
    private TriggerStayEvent m_OnTriggerStay = new TriggerStayEvent ();
 
    [SerializeField]
    private TriggerExitEvent m_OnTriggerExit = new TriggerExitEvent ();
 
 
    public CollisionEnterEvent OnCollisionEnterEvent
    {
        get { return m_OnCollisionEnter; }
        set { m_OnCollisionEnter = value; }
    }
 
    public CollisionStayEvent OnCollisionStayEvent
    {
        get { return m_OnCollisionStay; }
        set { m_OnCollisionStay = value; }
    }
 
    public CollisionExitEvent OnCollisionExitEvent
    {
        get { return m_OnCollisionExit; }
        set { m_OnCollisionExit = value; }
    }
 
    public TriggerEnterEvent OnTriggerEnterEvent
    {
        get { return m_OnTriggerEnter; }
        set { m_OnTriggerEnter = value; }
    }
 
    public TriggerStayEvent OnTriggerStayEvent
    {
        get { return m_OnTriggerStay; }
        set { m_OnTriggerStay = value; }
    }
 
    public TriggerExitEvent OnTriggerExitEvent
    {
        get { return m_OnTriggerExit; }
        set { m_OnTriggerExit = value; }
    }
 
    void OnCollisionEnter (Collision col)
    {
       m_OnCollisionEnter.Invoke (col);
    }

    void OnCollisionStay (Collision col)
    {
       m_OnCollisionStay.Invoke (col);
    }

    void OnCollisionExit (Collision col)
    {
       m_OnCollisionExit.Invoke (col);
    }

    void OnTriggerEnter (Collider col)
    {
       m_OnTriggerEnter.Invoke (col);
    }

    void OnTriggerStay (Collider col)
    {  
       m_OnTriggerStay.Invoke (col);
    }

    void OnTriggerExit (Collider col)
    {
       m_OnTriggerExit.Invoke (col);
    }
}