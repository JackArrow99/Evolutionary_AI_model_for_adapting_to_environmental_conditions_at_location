# Эволюционная модель ИИ для адаптации к условиям окружающей среды на локации
Данная работа является эволюционной моделью, построенной в среде игрового движка Unity при помощи инструментария Unity ML-Agents. Эволюционная модель дополнена элементами, позволяющими обучать для игр жанра "гонки" ИИ, адаптирующийся к условиям окружающей среды на локации. Обучение проходит с использованием методик Imitation Learning и набора записанных демонстраций.

Проект является частью магистерской выпускной квалификационной работы по образовательной программе "Технологии разработки компьютерных игр" Школы разработки видеоигр Университета ИТМО. Работа написана на языке C# в игровом движке Unity.

# Обзор
Эволюционная модель представляет собой построенный в соответствии с приведённой ниже архитектурой инструмент. При построении могут быть использованы программные средства, выполняющие соответствующие функции блоков архитектуры. В данной работе для построения модели в качестве среды обучения использовался игровой движок Unity, а в качестве нейросети - Unity ML-Agents. Качественная оценка производилась непосредственно с учётом опыта игрока.\
   ![Image alt](https://github.com/JackArrow99/Evolutionary_AI_model_for_adapting_to_environmental_conditions_at_location/raw/main/pictures/architecture.png)\
   *Созданная архитектура эволюционной модели* \
   \
   \
Среда обучения в движке Unity была создана для обучения ИИ для игр жанра "гонки".\
   \
   ![Image alt](https://github.com/JackArrow99/Evolutionary_AI_model_for_adapting_to_environmental_conditions_at_location/raw/main/pictures/track.png)\
   *Среда обучения агента - гоночный трек*\
   \
   \
   ![Image alt](https://github.com/JackArrow99/Evolutionary_AI_model_for_adapting_to_environmental_conditions_at_location/raw/main/pictures/learning_environment.png)\
   *Элементы среды обучения агента*\
   \
   \
   На сцене Unity размещён гоночный трек, состоящий из следующих компонентов:
1. Агент (машина);
2. Визуальная часть трека;
3. Стены трека;
4. Чекпоинты;
5. Зоны отличающейся поверхности трека.
<br />  
Для отслеживания состояния среды агентом использовалось два вида пространственных сенсоров: сенсоры границ трека и сенсоры поверхности.\
\
   ![Image alt](https://github.com/JackArrow99/Evolutionary_AI_model_for_adapting_to_environmental_conditions_at_location/raw/main/pictures/sensors_track.png)\
   *Пространственные сенсоры*\
   \
   \
   ![Image alt](https://github.com/JackArrow99/Evolutionary_AI_model_for_adapting_to_environmental_conditions_at_location/raw/main/pictures/sensors_surface.png)\
   *Сенсоры поверхности*\
   \
   \
При помощи Unity ML-Agents и Imitation Learning агент обучался проходить трек, преодолевая максимальное число чекпоинтов по очереди, избегая при этом столкновения со стенами и отличающимся типом поверхности ("песком").

# Запуск и использование
Работа является проектом, разработанным в Unity. Для запуска необходимо скачать репозиторий и открыть его через Unity Hub. Собранная тестовая сцена находится по пути ***Assets/Scenes/SampleScene.unity*** и содержит среду обучения из Обзора.

## Основные компоненты
Для обучения нового экземпляра ИИ для игры жанра "гонки" небходимо создать на сцене объект игрока и подключить его контроллер к агенту, добавив ссылку на контроллер в соответствующее поле. Подключение контроллера, отличного от приведённого в работе, потребует модификации скрипта ***CarAgent***. В скрипте должна быть добавлена возможность управлять конроллером при помощи функции *Heuristics*. После подключения контроллера необходимо провести настройку компонентов ***Behavior Parameters***, ***Decigion Requester***, а также обоих видов ***Ray Perception Sensor*** в соответствии с игровыми задачами симуляции. Подробнее о настройке компонентов Unity ML-Agents можно прочесть на официальном сайте: [Unity ML-Agents Toolkit](https://unity-technologies.github.io/ml-agents/)\
\
   ![Image alt](https://github.com/JackArrow99/Evolutionary_AI_model_for_adapting_to_environmental_conditions_at_location/raw/main/pictures/car_agent_script.png)\
   *Конфигурация компонента Behavior Parameters*
   

## Запись демонстраций с использованием Imitation Learning
Перед началом обучения необходимо записать демонстрацию при помощи компонента ***Demonstration Recorder***. Для этого его нужно перевести в режим записи, отметив флажком поле "Record". Поле "Num Steps To Record" заполняется в соответствии с игровыми задачами симуляции. Поле *Demonstration Name* заполняется в соответствии с названием поведения, используемым при дальнейшем обучении. В поле *Demonstration Directory* указывается директория, в которую будут сохраняться будущие демонстрации. Также в компоненте ***Behavior Parameters*** необходимо выбрать в поле *Behavior Type* значение "Inference Only".\
   <рисунок с настройкой параметров>\
После выставления параметров для начала демонстрации в Unity необходимо запустить Play Mode. В этот момент игрок будет управлять контроллером при помощи функции *Heuristics*. Действия игрока будут соотноситься с получаемыми агентом значениями вознаграждений и заноситься в Q-таблицу. По окончании записи демонстрации в указанной ранее директории будет создан файл демонстрации с расширением *".demo"*.

## Создание конфигурационного файла
Для дальнейшего использования демонстрации необходимо создать конфигурационный файл с указанием параметров обучения. Шаблон файла может быть взят на официальном сайте Unity ML-Agents: [Training Configuration File](https://unity-technologies.github.io/ml-agents/Training-Configuration-File/) Функции параметров описаны там же. При выборе значений параметров следует руководствоваться игровыми задачами симуляции. Созданный файл необходимо поместить в директорию ../Config.

## Обучение
Для запуска обучения необходимо при помощи командной оболочки запустить виртуальную среду обучения ML-Agents. Для этого нужно перейти в директорию проекта и выполнить следующую команду:
```
venv\Scripts\activate
```
Для запуска Unity ML-Agents нужно выполнить следующую команду:
```
mlagents-learn Config\Test_Config.yaml --run-id=Test
```
Перед началом обучения Unity в компоненте ***Behavior Parameters*** необходимо выбрать в поле *Behavior Type* значение "Default". После этого необходимо необходимо запустить Play Mode; начнётся процесс обучения агента. Для завершения процесса обучения агента остановите Play Mode. После этого в директории ../results будет создана директория, с названием, соответствующим *run-id*, и содржащая файл нейросети с расширением *".onnx"*.

## Использование результатов
Для использования созданного экземпляра ИИ (полученной нейросети) необходимо добавить в файлы проекта файл *".onnx"*. Для этого переместите файл в директорию ../Asset. В окне компонента ***Behavior Parameters*** в поле *Model* необходимо установить в качестве значения модели добавленную нейросеть. При запуске Play Mode нейросеть будет управлять действиями агента. Полученный таким образом ИИ может быть качественно оценён, и при положительном результате использован в качестве игрового ИИ.

# Дополнительно
Автор: Андреев Сергей\
Полный текст работы можно найти [на Github](https://github.com/JackArrow99/Evolutionary_AI_model_for_adapting_to_environmental_conditions_at_location/tree/main).
