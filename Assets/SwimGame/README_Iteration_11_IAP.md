# README — Iteration 11 — In-App Purchase: Coin Pack

## 0. ВАЖНО: установить пакет ДО импорта скриптов
Скрипты ссылаются на UnityEngine.Purchasing — без пакета проект не скомпилируется.
1. Package Manager -> "+" -> Add package by name -> `com.unity.purchasing`, версия `5.2.1`
2. Project Settings -> Services: проект должен быть привязан к Unity Project ID (иначе IAP не инициализируется)
3. Services -> In-App Purchasing -> включить
4. Открыть IAP Catalog (Services -> In-App Purchasing -> IAP Catalog):
   - Add Product: ID `com.coinspacksmall.inapp`, Type: **Consumable**
   - Включить "Automatically initialize UnityPurchasing (recommended)"
5. В App Store Connect продукт с этой же айдишкой должен быть заведён как Consumable (для теста на устройстве — sandbox-аккаунт)

## 1. Что было добавлено
- **Кнопка покупки монет в главном меню** ("+500 COINS", внизу по центру) через Codeless IAP Button (CodelessIAPButton). Никакой ручной инициализации — codeless сам поднимает UnityPurchasing
- **CoinPackIAP** — обработчики трёх событий кнопки:
  - `onProductFetched` -> в кнопке появляется локализованная цена из стора (до этого "...")
  - `onPurchaseComplete` -> +500 монет (CurrencyManager), звук, вибрация, разлёт монет к счётчику, punch кнопки
  - `onPurchaseFailed` -> звук отказа, тряска кнопки, warning с причиной в консоль
- **Счётчик монет в главном меню** (top-right) — результат покупки виден сразу

## 2. Что изменилось с прошлой итерации
**ЗАМЕНИТЬ файлы:**
- `Scripts/UI/MainMenuUI.cs` — счётчик монет (поля coinsText/coinIcon + подписка на баланс)

**НОВЫЕ файлы:** CoinPackIAP.cs (UI); Iteration11Builder.cs (Editor).

Количество монет в паке и айдишка — константы в CoinPackIAP (CoinsAmount = 500, ProductId = "com.coinspacksmall.inapp").

## 3. Editor скрипты — какие меню запустить и в каком порядке
1. Сначала шаги из пункта 0 (пакет + каталог)!
2. `SwimGame -> Update Main Menu Scene (Iteration 11)` — счётчик монет + кнопка пака с CodelessIAPButton (productId и Button прокинуты автоматически)

## 4. Как тестировать
1. **В редакторе:** Unity IAP использует FakeStore — нажатие на "+500 COINS" сразу даёт успешную покупку: +500 монет, монеты летят к счётчику, баланс докручивается. Цена покажется как фейковая ($0.01)
2. Купить дважды — баланс растёт на 500 каждый раз (Consumable)
3. Зайти в SHOP — на 500 монет реально закупиться предметами
4. **На устройстве (sandbox):** цена в кнопке подтянется из App Store (onProductFetched), системный диалог покупки, отмена покупки -> тряска кнопки и звук отказа, без начисления
5. Перезапуск — купленные монеты на месте (PlayerPrefs)

## 5. Ожидаемый результат
Рабочая петля монетизации: не хватает монет на предметы -> покупка пака в меню -> магазин. Все три события IAP-кнопки обработаны с фидбеком.

## 6. Известные ограничения
- Один пак (small). Добавление medium/large — копия кнопки с другой айдишкой и константой
- Серверной валидации чека нет (для продакшена стоит добавить receipt validation)
- Restore-кнопка не нужна (consumable), но потребуется при добавлении non-consumable товаров

## 7. Дальше
По желанию: паки medium/large, валидация чеков, попап магазина монет вместо одиночной кнопки.
