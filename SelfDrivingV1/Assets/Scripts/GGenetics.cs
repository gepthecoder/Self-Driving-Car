using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using System;

public class GGenetics : MonoBehaviour
{
    [SerializeField] private CarController m_CController;

    [Header("CONTROLS")]
    [Space(10)]
    [Tooltip("Total Initial Population")]
    [SerializeField] private int m_InitialPopulation = 85;
    [Tooltip("Chance that the network will be randomized, so that we are not constrained to the initial population that was created")]
    [SerializeField] [Range(0f, 1f)] private float m_MutationRate = .055f;
    [Space(30)]
    [Header("Crossover Controls")]
    [Space(10)]
    [Tooltip("How and What method the two parents are morphed/combined together. Same goes for gnetworks ")]
    [SerializeField] private int m_BestAgentSelection = 8;
    [SerializeField] private int m_WorstAgentSelection = 3;
    [Tooltip("Example: pick 8 from the top & 3 from the bottom (best/worst performes) - crossage")]
    [SerializeField] private int m_NumberToCrossover;

    // Contains All The Networks That Have Been Selected
    private List<int> m_GGenPool = new List<int>();

    private int m_NaturallySelected; // How Many Have Been Actually Selected And How Many Do We Need To Randomly Generate

    private NeuralNetwork[] m_Population;

    [Space(30)]
    [Header("Debug")]
    [Space(10)]
    // In Each Generation Is There are 200 Genoms For Example And The Genoms Are Each Individual Car In That Popoulation
    // Once We Iterate Through Every Car In That Population We Increment The Generation and We Go To The Next Generation
    // For Tracking How Good The Genetic Algorithm Is Doing -> The Lower Amount the Generation it Take The Quicker It Is Evolving.
    [SerializeField] private int m_CurrentGeneration;
    [SerializeField] private int m_CurrentGenome;

    private void Start()
    {
        CreatePopulation();
    }

    private void CreatePopulation()
    {
        m_Population = new NeuralNetwork[m_InitialPopulation];
        RandomlyFillPopulation(m_Population, 0);
        ResetToCurrentGenome();
    }

    private void ResetToCurrentGenome()
    {
        m_CController.ResetWithNNetwork(m_Population[m_CurrentGenome]);
    }

    private void RandomlyFillPopulation(NeuralNetwork[] newPopulation, int startingIndex)
    {
        while (startingIndex < m_InitialPopulation)
        {
            newPopulation[startingIndex] = new NeuralNetwork();
            newPopulation[startingIndex].Init(m_CController.GetNNetworkLayersCount(), m_CController.GetNNetworkNeuronsCount());

            startingIndex++;
        }
    }

    public void Death(float fitness, NeuralNetwork nNet)
    {
        if(m_CurrentGenome < m_Population.Length - 1)
        {
            m_Population[m_CurrentGenome].SetFitness(fitness);

            // Go To Next Car In Out Current Generation
            m_CurrentGenome++;
            ResetToCurrentGenome();
        } else
        {
            // Repopulate
        }
    }
}
