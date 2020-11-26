using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    /* game config data */
    string[] topicList = new string[] { "People", "Country", "Professtion" };
    string[][] wordLists =
    {
        new string[] {"Isaac Newton", "Albert Einstein", "Abraham Lincoln", "Donald Trump", "Bill Gates","Muhammad Ali", "Mother Teresa", "Leonardo da Vinci", "Thomas Edison", "Neil Armstrong"},
        new string[] {"Vietnam", "America", "Poland", "Afghanistan", "Belgium", "Argentina","Cambodia","Egypt","Ethiopia","Kyrgyzstan"},
        new string[] { "Pharmacist", "Photojournalist", "Barber", "Accountant","Optometrist", "Arborist","Astrophysicist","Gunsmiths","Brewer","Imam"}
    };
    const int HIDDEN_FACTOR = 3;     // 1/part of word length that will be hidden
    const char HIDDEN_CHAR = '*';  // charater used to hide characters of the word
    const int MAX_ATTEMP = 3;

    /* game state */
    enum Screen { MAIN_MENU, GAME_PLAY, GAME_OVER };
    Screen screen;
    int currentTopic;
    string[] currentWordList;
    int currentWordIndex;
    int attempCount;
    int score;

    // Start is called before the first frame update
    void Start()
    {
        ShowMainMenu();
    }

    void ShowMainMenu()
    {
        screen = Screen.MAIN_MENU;
        Terminal.ClearScreen();
        Terminal.WriteLine("Welcome to Guess the Word!\n\n" +
            "Choose a topic:");
        for (int i = 0; i < topicList.Length; i++)
        {
            Terminal.WriteLine((i + 1) + ". " + topicList[i]);
        }
        Terminal.WriteLine("Enter your number: ");
    }

    void ShowTitle()
    {
        Terminal.ClearScreen();
        Terminal.WriteLine("Topic: " + topicList[currentTopic - 1]);
        Terminal.WriteLine("(type 'menu' to return to main menu)\n");
    }

    void OnUserInput(string input)
    {
        if (input == "menu")
        {
            ShowMainMenu();
        }
        else
        {
            switch (screen)
            {
                case Screen.MAIN_MENU:
                    HandleMainMenuInput(input);
                    break;
                case Screen.GAME_PLAY:
                    CheckWord(input);
                    break;
            }
        }
    }

    void CheckWord(string input)
    {
        if (input.Equals(currentWordList[currentWordIndex], StringComparison.InvariantCultureIgnoreCase))
        {
            score++;
            Terminal.WriteLine("Correct answer!");
            // delay for 1s   
            Invoke("ShowNextWord", (float)0.5);
        }
        else
        {
            if (attempCount < MAX_ATTEMP-1)
            {
                attempCount++;
                Terminal.WriteLine("Wrong answer! You can try " + (MAX_ATTEMP - attempCount) + " more times");
            }
            else
            {
                ShowNextWord();
            }
        }
    }

    void ShowNextWord()
    {
        if (currentWordIndex == currentWordList.Length - 1)
        {
            ShowGameOver();
        }
        else
        {
            attempCount = 0;
            currentWordIndex++;
            ShowWord(currentWordList[currentWordIndex]);
        }
    }

    void HandleMainMenuInput(string input)
    {
        int inputInt;

        // if input can be parsed as int
        if (Int32.TryParse(input, out inputInt) && inputInt <= topicList.Length)
        {
            currentTopic = inputInt;
            score = 0;
            screen = Screen.GAME_PLAY;
            currentTopic = Int32.Parse(input);
            currentWordList = wordLists[currentTopic - 1];
            PlayGame();

        }
        else
        {
            Terminal.WriteLine("Invalid option!");
        }
    }

    void ShowGameOver()
    {
        Terminal.ClearScreen();
        Terminal.WriteLine("Congratulation!\n" +
            "You have " + score + "/" + currentWordList.Length + " correct answer(s)");
        Invoke("ShowMainMenu", (float)1.5);
    }

    void ShowWord(string word)
    {
        ShowTitle();

        char[] formattedWord = word.ToUpper().ToCharArray();

        var rand = new System.Random();
        List<int> hiddenPositions = new List<int>();

        // hide 2.5 length of the current word
        hiddenPositions.AddRange(Enumerable.Range(0, formattedWord.Length - 1)
                               .OrderBy(i => rand.Next())
                               .Take((int)(formattedWord.Length / HIDDEN_FACTOR)));
        foreach (int index in hiddenPositions)
        {
            if (formattedWord[index] != ' ')
            {
                formattedWord[index] = HIDDEN_CHAR;
            }
        }

        Terminal.WriteLine(new string(formattedWord));
    }

    void PlayGame()
    {
        Terminal.ClearScreen();
        Shuffle(currentWordList);
        currentWordIndex = 0;
        ShowWord(currentWordList[currentWordIndex]);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void Shuffle<T>(T[] items)
    {
        System.Random rand = new System.Random();

        // For each spot in the array, pick
        // a random item to swap into that spot.
        for (int i = 0; i < items.Length - 1; i++)
        {
            int j = rand.Next(i, items.Length);
            T temp = items[i];
            items[i] = items[j];
            items[j] = temp;
        }
    }
}


