# Pinester

## Создание базы данных
```
CREATE TABLE images (
    id SERIAL PRIMARY KEY,          -- Уникальный идентификатор изображения (автоинкремент)
    file_name VARCHAR(255) NOT NULL, -- Имя файла
    mime_type VARCHAR(100),         -- MIME-тип (например, image/jpeg, image/png)
    image_data BYTEA NOT NULL,      -- Бинарные данные изображения
    uploaded_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP -- Дата загрузки
);
```
