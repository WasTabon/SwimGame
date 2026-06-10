# README — Iteration 01 — Menu and Foundation

## 1. Что было добавлено
Главное меню с кнопками PLAY / SETTINGS / SHOP, пустая игровая сцена с кнопкой BACK, и весь фундамент: SoundManager (пул из 6 AudioSource, программно сгенерированные placeholder-звуки и фоновая музыка), HapticManager, TransitionManager (fade-переход между сценами), SafeAreaFitter, ButtonAnimator (scale punch + звук на каждой кнопке), GameBootstrap (60 FPS, DOTween init).

## 2. Что изменилось с прошлой итерации
Это первая итерация, изменений нет.

## 3. Перед запуском (один раз)
1. Установить DOTween Free из Asset Store, затем Tools → Demigiant → DOTween Utility Panel → Setup DOTween
2. Импортировать TMP Essentials: Window → TextMeshPro → Import TMP Essential Resources
3. Скопировать папку `Assets/SwimGame` из архива в проект

## 4. Editor скрипты — какие меню запустить и в каком порядке
1. `SwimGame → Build Main Menu Scene (Iteration 1)` — создаст и сохранит сцену MainMenu, добавит её в Build Settings первой
2. `SwimGame → Build Game Scene (Iteration 1)` — создаст и сохранит сцену Game, добавит её в Build Settings

Сцены сохраняются в `Assets/SwimGame/Scenes/`.

## 5. Как тестировать
1. Открыть сцену MainMenu и нажать Play в редакторе
2. Нажать кнопку PLAY — должен произойти fade-переход на сцену Game
3. На сцене Game нажать BACK — fade-переход обратно в меню
4. Понажимать SETTINGS и SHOP — кнопки анимируются и звучат, но ничего не открывают (это нормально)

## 6. Ожидаемый результат
Тёмно-синий фон (#1A1A2E), заголовок SWIM GAME сверху, оранжевая кнопка PLAY по центру, две синие кнопки ниже. Каждое нажатие кнопки: scale punch 0.95 → 1.0 и короткий звук "tap". Играет тихая фоновая эмбиент-музыка (синтетический placeholder). Переходы между сценами через плавное затемнение, во время перехода клики блокируются.

## 7. Известные ограничения текущей итерации
- SETTINGS и SHOP не функциональны (попапы в итерациях 8-9)
- Сцена Game пустая, только заголовок и BACK
- Все звуки — синтетические синусоиды-плейсхолдеры
- Haptics подключены, но пока нигде не вызываются

## 8. Что будет в следующей итерации
Сетка, загрузка уровней из ScriptableObject, игрок и движение тапом по соседним клеткам с подсветкой.
