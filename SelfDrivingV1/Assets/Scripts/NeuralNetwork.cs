using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class NeuralNetwork : MonoBehaviour
{
    [SerializeField] private Matrix<float> m_InputLayer = Matrix<float>.Build.Dense(1, 3);
    [SerializeField] private List<Matrix<float>> m_HiddenLayers = new List<Matrix<float>>();

    [SerializeField] private Matrix<float> m_OutputLayer = Matrix<float>.Build.Dense(1, 2);

    [SerializeField] private List<Matrix<float>> m_Weights = new List<Matrix<float>>();
    [SerializeField] private List<float> m_Biases = new List<float>();

    [SerializeField] private float m_Fitness;

    public void Init(int hiddenLayerCount, int hiddenNeuronCount)
    {
        ClearData();

        for (int i = 0; i < hiddenLayerCount; i++)
        {
            Matrix<float> hiddenLayer = Matrix<float>.Build.Dense(1, hiddenNeuronCount);
            m_HiddenLayers.Add(hiddenLayer);

            m_Biases.Add(Random.Range(-1f, 1f));

            // Weights => Connect weights from previous layer to the current layer 

            if(i == 1) // INPUT LAYER
            {
                Matrix<float> inputToH1 = Matrix<float>.Build.Dense(3, hiddenLayerCount);
                m_Weights.Add(inputToH1);
            }

            Matrix<float> hiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuronCount, hiddenNeuronCount);
            m_Weights.Add(hiddenToHidden);

        }

        Matrix<float> outputWeight = Matrix<float>.Build.Dense(hiddenNeuronCount, 2);
        m_Weights.Add(outputWeight);

        m_Biases.Add(Random.Range(-1f, 1f));

        RandomizeWeights();
    }

    public (float, float) RunNetwork(float sensorA, float sensorB, float sensorC) 
    {
        m_InputLayer[0, 0] = sensorA;
        m_InputLayer[0, 1] = sensorB;
        m_InputLayer[0, 2] = sensorC;

        m_InputLayer = m_InputLayer.PointwiseTanh(); // Acceleration & Steering needs to be between -1 & 1 (WITHOUT losing data) -> wiseTanH

        // Manual Connection Between InputLayer AND the first Hidden Layer (weights) 
        m_HiddenLayers[0] = ((m_InputLayer * m_Weights[0]) + m_Biases[0]).PointwiseTanh();

        for (int i = 1; i < m_HiddenLayers.Count; i++)
        {
            m_HiddenLayers[i] = ((m_HiddenLayers[i - 1] * m_Weights[i]) + m_Biases[i]).PointwiseTanh();
        }

        // Assign Values to output layer
        m_OutputLayer = ((m_HiddenLayers[m_HiddenLayers.Count - 1] * m_Weights[m_Weights.Count - 1]) + m_Biases[m_Biases.Count - 1]).PointwiseTanh();

        // First output is ACCELERATION, second one is STEERING
        return (Sigmoid(m_OutputLayer[0,0]), (float)Math.Tanh(m_OutputLayer[0, 1]));
    }

    private float Sigmoid(float x) // Curve between 0 & 1
    {
        return (1 / (1 + Mathf.Exp(-x)));
    }

    private void RandomizeWeights()
    {
        for (int i = 0; i < m_Weights.Count; i++)
        {
            for (int j = 0; j < m_Weights[i].RowCount; j++)
            {
                for (int k = 0; k < m_Weights[i].ColumnCount; k++)
                {
                    m_Weights[i][j, k] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    private void ClearData()
    {
        m_InputLayer.Clear();
        m_HiddenLayers.Clear();
        m_OutputLayer.Clear();
        m_Weights.Clear();
        m_Biases.Clear();
    }

}
