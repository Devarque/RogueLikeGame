using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Mermilerin bir nesneye �arpt�ktan sonra, yok olmas�ndan sonra ortaya ��kan particle par�alar�n�n parlamas� i�in
//bu script particle nesnelerine aktar�l�r. Her bir particle par�as�na olu�tu�u andan yok oldu�u ana kadar particle ile
//ayn� boyutlarda birer nesne (kare sprite gibi)olu�turur ve bu nesne particle par�alar�na eklenir. Bu nesnelere 2d
//���k eklenerek particle par�alar�n�n parlamas� ve oyundaki post process i�lemlerinin de bu
//nesneler sayesinde particle par�alar�na dahil edilmesini sa�lar.
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Kaynak: https://forum.unity.com/threads/lwrp-using-2d-lights-in-a-particle-system-emitter.718847/#post-5554201

public class AttachGameObjectsToParticles : MonoBehaviour {
    public GameObject m_Prefab;

    private ParticleSystem m_ParticleSystem;
    private List<GameObject> m_Instances = new List<GameObject>();
    private ParticleSystem.Particle[] m_Particles;

    // Start is called before the first frame update
    void Start() {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
    }

    // Update is called once per frame
    void LateUpdate() {
        int count = m_ParticleSystem.GetParticles(m_Particles);

        while (m_Instances.Count < count)
            m_Instances.Add(Instantiate(m_Prefab, m_ParticleSystem.transform));

        bool worldSpace = (m_ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World);
        for (int i = 0; i < m_Instances.Count; i++) {
            if (i < count) {
                if (worldSpace)
                    m_Instances[i].transform.position = m_Particles[i].position;
                else
                    m_Instances[i].transform.localPosition = m_Particles[i].position;
                m_Instances[i].SetActive(true);
            } else {
                m_Instances[i].SetActive(false);
            }
        }
    }
}
