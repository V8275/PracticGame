using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    fishList fl;

    public RandomSpawn(fishList f) { fl = f; }

    public int RandomNumber()
    {
        // ����������� ��������������� ��������
        double[] probabilities = fl.GetPercents();

        System.Random r = new System.Random();
        // ��������� ���������� ����� �� 0 �� 1
        double randomValue = r.NextDouble();

        // ���������� ���������� ������� �� ������ ������������
        int selectedIndex = ChooseIndexWithProbability(probabilities, randomValue);

        return selectedIndex;
    }
    

    static int ChooseIndexWithProbability(double[] probabilities, double randomValue)
    {
        double cumulativeProbability = 0.0;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];

            if (randomValue < cumulativeProbability) return i;
        }

        // ��������, �� ����������, �� ���� randomValue = 1.0
        // � ��� ����������� �����, ������� ��������� ������
        return probabilities.Length - 1;
    }
}