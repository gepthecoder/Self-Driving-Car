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

    /// <summary>
    /// Go through all the nets that aren't initialized yet (in the sense that they were not crosseover yet nor added as a best network)
    /// </summary>
    /// <param name="newPopulation"></param>
    /// <param name="startingIndex"></param>
    private void RandomlyFillPopulation(NeuralNetwork[] newPopulation, int startingIndex)
    {
        while (startingIndex < m_InitialPopulation)
        {
            newPopulation[startingIndex] = new NeuralNetwork();
            newPopulation[startingIndex].Init(m_CController.GetNNetworkLayersCount(), m_CController.GetNNetworkNeuronsCount());

            startingIndex++;
        }
    }

    /// <summary>
    /// Go To Next Car In Out Current Generation
    /// </summary>
    /// <param name="fitness"></param>
    /// <param name="nNet"></param>
    public void Death(float fitness, NeuralNetwork nNet)
    {
        if(m_CurrentGenome < m_Population.Length - 1)
        {
            m_Population[m_CurrentGenome].SetFitness(fitness);

            m_CurrentGenome++;
            UiManager.instance.SetCurrentGenomeText(m_CurrentGenome.ToString());
            ResetToCurrentGenome();
        } else
        {
            // Repopulate
            Repopulate();
        }
    }

    /// <summary>
    /// Sort the population by fitness (the higher the fitness the higher on the ggen pool)
    /// </summary>
    private void Repopulate()
    {
        m_GGenPool.Clear();

        m_CurrentGeneration++;
        UiManager.instance.SetCurrentGenerationText(m_CurrentGeneration.ToString());

        m_NaturallySelected = 0;

        SortPopulation();

        NeuralNetwork[] nNet = PickBestPopulation();

        CrossoverPopulation(nNet);
        MutatePopulation(nNet);

        RandomlyFillPopulation(nNet, m_NaturallySelected);

        m_Population = nNet;

        m_CurrentGenome = 0;
        UiManager.instance.SetCurrentGenomeText(m_CurrentGenome.ToString());

        ResetToCurrentGenome();
    }

    /// <summary>
    /// Pick the best and worst performers and cross them over.
    /// </summary>
    /// <param name="nNet"></param>
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

            // Swap over the entire WEIGHTS of matrices
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

            // Swap over the entire BIASES of matrices
            for (int k = 0; k < child1.GetBiases().Count; k++)
            {
                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    child1.GetBiases()[k] = m_Population[firstParentIndex].GetBiases()[k];
                    child2.GetBiases()[k] = m_Population[secondParentIndex].GetBiases()[k];
                }
                else
                {
                    child2.GetBiases()[k] = m_Population[firstParentIndex].GetBiases()[k];
                    child1.GetBiases()[k] = m_Population[secondParentIndex].GetBiases()[k];
                }
            }

            nNet[m_NaturallySelected] = child1;
            m_NaturallySelected++;

            nNet[m_NaturallySelected] = child2;
            m_NaturallySelected++;
        }
    }

    /// <summary>
    /// Mixing the Gen Pool -> Without it [if the initial population wasn't good -> no mather how many crossovers you do, you might never get a result that is satesfactory ----> WE DO MUTATIONS]
    /// Mutation Rate is set to very LOW so we don't mutate that often ("good" ones aren't maybe that good)
    /// sThe children are getting mutated
    /// </summary>
    /// <param name="nNet"></param>
    private void MutatePopulation(NeuralNetwork[] nNet)
    {
        // Looping only through naturally selected population cuz the rest of the population wasn't even initialized yet so why would we mutate?
        for (int i = 0; i < m_NaturallySelected; i++)
        {
            for (int j = 0; j < nNet[i].GetWeights().Count; j++)
            {
                if (Random.Range(0.0f, 1.0f) < m_MutationRate)
                {
                    nNet[i].GetWeights()[j] = MutateMatrix(nNet[i].GetWeights()[j]);
                }
            }
        }
    }

    private Matrix<float> MutateMatrix(Matrix<float> matrix)
    {
        //int randomPickingFactor = Random.Range(1, 9);

        // Insert come random amount of values inside passed matrix to make them change
        int randomPoints = Random.Range(1, (matrix.RowCount * matrix.ColumnCount) / 7);

        Matrix<float> newMatrix = matrix;

        for (int i = 0; i < randomPoints; i++)
        {
            int randomColumn = Random.Range(0, newMatrix.ColumnCount);
            int randomRow = Random.Range(0, newMatrix.RowCount);

            // Bumb Up or Down a bit
            newMatrix[randomRow, randomColumn] = Mathf.Clamp(newMatrix[randomRow, randomColumn] + Random.Range(-1,1), -1f, 1f);
        }

        return newMatrix;
    }


    /// <summary>
    /// Tip: We dont wanna LOSE: sometimes we would find the best car that works amazing but through some crossover it can actually get worse. Ask yourselft Why Would you wanna lose that agent that did amazing ??
    /// </summary>
    /// <returns></returns>
    private NeuralNetwork[] PickBestPopulation()
    {
        NeuralNetwork[] newPopulation = new NeuralNetwork[m_InitialPopulation];

        // Best Performers
        for (int i = 0; i < m_BestAgentSelection; i++)
        {
            // assign the new population naturally selected to the best agent [transfering the best networks from the current generation over to the next generation unharmed]
            // we dont wanna lose the best networks - so we are passing in the best networks
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

            int tempf = Mathf.RoundToInt(m_Population[last].GetFitness() * 10);

            // We are not increasing the natural selected cuz we are not bringing over the worst agents/genoms to the next gen

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
