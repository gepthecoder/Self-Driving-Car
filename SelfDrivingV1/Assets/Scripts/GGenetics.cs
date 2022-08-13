using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using System;

using Random = UnityEngine.Random;

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
            Repopulate();
        }
    }

    private void Repopulate()
    {
        m_GGenPool.Clear();

        m_CurrentGeneration++;
        m_NaturallySelected = 0;

        // Sort the population by fitness (the higher the fitness the higher on the ggen pool)
        SortPopulation();

        NeuralNetwork[] nNet = PickBestPopulation();

        CrossoverPopulation(nNet);
        MutatePopulation(nNet);
    }

    private void CrossoverPopulation(NeuralNetwork[] nNet)
    {
        for (int i = 0; i < m_NumberToCrossover; i+=2)
        {
            int firstParentIndex = i;
            int secondParentIndex = i + 1;

            if(m_GGenPool.Count >= 1)
            {
                for (int j = 0; j < 100; j++)
                {
                    firstParentIndex = m_GGenPool[Random.Range(0, m_GGenPool.Count)];
                    secondParentIndex = m_GGenPool[Random.Range(0, m_GGenPool.Count)];

                    if(firstParentIndex != secondParentIndex)
                    {
                        // we found potential parents
                        break;
                    }
                }
            }
            NeuralNetwork child1 = new NeuralNetwork();
            NeuralNetwork child2 = new NeuralNetwork();

            child1.Init(m_CController.GetNNetworkLayersCount(), m_CController.GetNNetworkNeuronsCount());
            child2.Init(m_CController.GetNNetworkLayersCount(), m_CController.GetNNetworkNeuronsCount());

            child1.SetFitness(0);
            child2.SetFitness(0);

            // Swap over the entire weights of matrices
            for (int k = 0; k < child1.GetWeights().Count; k++)
            {
                if(Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    child1.GetWeights()[k] = m_Population[firstParentIndex].GetWeights()[k];
                    child2.GetWeights()[k] = m_Population[secondParentIndex].GetWeights()[k];
                }
                else
                {
                    child2.GetWeights()[k] = m_Population[firstParentIndex].GetWeights()[k];
                    child1.GetWeights()[k] = m_Population[secondParentIndex].GetWeights()[k];
                }
            }
        }
    }
    private void MutatePopulation(NeuralNetwork[] nNet)
    {
        throw new NotImplementedException();
    }


    private NeuralNetwork[] PickBestPopulation()
    {
        NeuralNetwork[] newPopulation = new NeuralNetwork[m_InitialPopulation];

        // Best Performers
        for (int i = 0; i < m_BestAgentSelection; i++)
        {
            newPopulation[m_NaturallySelected] = m_Population[i].InitializeCopy(m_CController.GetNNetworkLayersCount(), m_CController.GetNNetworkNeuronsCount());
            newPopulation[m_NaturallySelected].SetFitness(0);
            m_NaturallySelected++;

            int tempf = Mathf.RoundToInt(newPopulation[i].GetFitness() * 10);

            for (int j = 0; j < tempf; j++)
            {
                // add index (instead of refs - memory efficieny) of the new population
                m_GGenPool.Add(i);
            }
        }

        // Worst Performers - inverse loop
        for (int i = 0; i < m_WorstAgentSelection; i++)
        {
            int last = m_Population.Length - 1;
            last -= i;

            int tempf = Mathf.RoundToInt(newPopulation[last].GetFitness() * 10);

            for (int j = 0; j < tempf; j++)
            {
                // add index (instead of refs - memory efficieny) of the new population
                m_GGenPool.Add(last);
            }
        }

        return newPopulation;

    }

    private void SortPopulation()
    {
        // Quick Sort
        for (int i = 0; i < m_Population.Length; i++)
        {
            for (int j = i; j < m_Population.Length; j++)
            {
                if(m_Population[i].GetFitness() < m_Population[j].GetFitness())
                {
                    NeuralNetwork temp = m_Population[i];
                    m_Population[i] = m_Population[j];
                    m_Population[j] = temp;
                }
            }
        }
    }
}
