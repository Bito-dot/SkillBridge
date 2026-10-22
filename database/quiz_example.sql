USE skillbridge;

START TRANSACTION;

INSERT INTO courses (category_id,title,normalized_title,summary,description,level,is_published)
SELECT id,'SQL Quiz Practice','SQL QUIZ PRACTICE',
       'Practise two basic SQL statements with a short quiz.',
       'An example course for checking the quiz workflow. Read the questions, choose an answer and review the explanation.',
       'Beginner',TRUE
FROM categories
WHERE NOT EXISTS (SELECT 1 FROM courses WHERE normalized_title='SQL QUIZ PRACTICE')
ORDER BY id LIMIT 1;
SET @example_course = IF(ROW_COUNT()=1,LAST_INSERT_ID(),NULL);

INSERT INTO quizzes (course_id,title,instructions,pass_percentage,is_published)
SELECT @example_course,'SQL basics quiz','Choose one answer for each question.',80,TRUE
WHERE @example_course IS NOT NULL;
SET @example_quiz = IF(@example_course IS NOT NULL,LAST_INSERT_ID(),NULL);

INSERT INTO questions (quiz_id,question_text,explanation,marks,sequence_number)
SELECT @example_quiz,'Which SQL statement reads rows from a table?',
       'SELECT reads data. INSERT adds rows, UPDATE changes rows and DELETE removes rows.',3,1
WHERE @example_quiz IS NOT NULL;
SET @first_question = IF(@example_quiz IS NOT NULL,LAST_INSERT_ID(),NULL);

INSERT INTO options (question_id,option_text,sequence_number,is_correct)
SELECT @first_question,'SELECT',1,TRUE WHERE @first_question IS NOT NULL
UNION ALL SELECT @first_question,'INSERT',2,FALSE WHERE @first_question IS NOT NULL
UNION ALL SELECT @first_question,'UPDATE',3,FALSE WHERE @first_question IS NOT NULL
UNION ALL SELECT @first_question,'DELETE',4,FALSE WHERE @first_question IS NOT NULL;

INSERT INTO questions (quiz_id,question_text,explanation,marks,sequence_number)
SELECT @example_quiz,'Which SQL statement adds a new row to a table?',
       'INSERT adds a row to a table. SELECT only reads existing data.',1,2
WHERE @example_quiz IS NOT NULL;
SET @second_question = IF(@example_quiz IS NOT NULL,LAST_INSERT_ID(),NULL);

INSERT INTO options (question_id,option_text,sequence_number,is_correct)
SELECT @second_question,'SELECT',1,FALSE WHERE @second_question IS NOT NULL
UNION ALL SELECT @second_question,'UPDATE',2,FALSE WHERE @second_question IS NOT NULL
UNION ALL SELECT @second_question,'INSERT',3,TRUE WHERE @second_question IS NOT NULL
UNION ALL SELECT @second_question,'DELETE',4,FALSE WHERE @second_question IS NOT NULL;

COMMIT;
