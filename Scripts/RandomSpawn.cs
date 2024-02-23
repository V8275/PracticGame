using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    fishList fl;

    public RandomSpawn(fishList f) { fl = f; }

    public int RandomNumber()
    {
        // Вероятности соответствующих индексов
        double[] probabilities = fl.GetPercents();

        System.Random r = new System.Random();
        // Получение случайного числа от 0 до 1
        double randomValue = r.NextDouble();

        // Вычисление выбранного индекса на основе вероятностей
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

        // Вероятно, не достигнуто, но если randomValue = 1.0
        // и все вероятности равны, вернуть последний индекс
        return probabilities.Length - 1;
    }
}