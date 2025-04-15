document.addEventListener('DOMContentLoaded', function () {
    const questionTemplate = document.getElementById("question-template");
    const questionContainer = document.getElementById("question-container");
    const createQuestionBtn = document.getElementById("create-question");
    const deleteQuestionBtn = document.getElementById("delete-question");
    const selectAllBtn = document.getElementById("select-all");
    let questionCount = document.querySelectorAll(".question-item").length;
    let isCheck = true;
 

    createQuestionBtn.addEventListener("click", (e) => {
        e.preventDefault();
        createQuestion();
    });
    deleteQuestionBtn.addEventListener('click', (e) => {
        e.preventDefault();
        removeQuestions();
    });
    selectAllBtn.addEventListener("click", (e) => {
        e.preventDefault();
        selectAllQuestions();
    });
    function selectAllQuestions() {
        const questions = questionContainer.querySelectorAll(".question-item");
        questions.forEach(question => {
            const input = question.querySelector(".check-wrap input");
            input.checked = isCheck;
            toggleBorder(isCheck, question);
        })
        isCheck = !isCheck;
    }
    function removeQuestions() {
        const questions = questionContainer.querySelectorAll(".question-item");
        questions.forEach(question => {
            if (question.querySelector(".check-wrap input").checked) {
                question.remove();
                questionCount--;
            }
        })
        updateIndexes();
    }
    function updateIndexes() {
        const questions = questionContainer.querySelectorAll(".question-item");
        questions.forEach((question, index) => {
            question.querySelectorAll('[name^="Questions["]').forEach(item => {
                item.name = item.name.replace(/\[\d+\]/g, `[${index}]`);
                item.dataset.id = index;
            });
        })
    }
    questionContainer.addEventListener("click", (e) => {
        if (e.target.classList.contains('question-item')) {
            const checkBox = e.target.querySelector(".check-wrap input");
            checkBox.checked = !checkBox.checked;
            toggleBorder(checkBox.checked, e.target)
        }
        else if (e.target.matches(".question-item .check-wrap input")) {
            const questionItem = e.target.closest(".question-item");
            toggleBorder(e.target.checked, questionItem);
        }
        else if (e.target.matches(".description-check")) {
            e.target.closest(".question-item").querySelector(".description-container").classList.toggle("d-none");
        }
        else if (e.target.closest(".add-option")) {
            console.log(e);
            e.preventDefault();
            const questionItem = e.target.closest(".question-item");
            const checkboxesContainer = questionItem.querySelector(".checkboxes-container");
            const optionIndex = checkboxesContainer.querySelectorAll("div").length;
            checkboxesContainer.appendChild(createCheckBox(questionItem.dataset.id, optionIndex));
        }
        else if (e.target.closest(".close-option")) {
            console.log("close");
            e.preventDefault();
            const container = e.target.closest(".checkboxes-container");
            e.target.closest('div').remove();
            reindexOptions(container);
        }
        
    });


    questionContainer.addEventListener("input", (e) => {
        if (e.target.matches(".question-type")) {
            const questionItem = e.target.closest(".question-item");
            const answerContainer = questionItem.querySelector(".answer-container");
            const selectValue = e.target.value;
            const questionIndex =questionItem.dataset.id;
            const answer = createAnswerElement(selectValue, questionIndex);
            answerContainer.innerHTML = "";
            answerContainer.appendChild(answer);
        }
    });

    function ChangeQuestionType(questionElement, questionIndex) {
        const questionType = questionElement.querySelector(".question-type");
        const answerContainer = questionElement.querySelector(".answer-container");
        answerContainer.appendChild(createAnswerElement(questionType.value, questionIndex));       
    }
    //questionType.addEventListener('input', (e) => {
    //    const selectValue = e.target.value;
    //    const answer = createAnswerElement(selectValue, questionIndex);
    //    answerContainer.innerHTML = "";
    //    answerContainer.appendChild(answer);
    //});
    function toggleBorder(isChecked, questionItem) {
        if (isChecked) {
            questionItem.classList.add("border-warning");
            questionItem.classList.remove("border")
            questionItem.classList.add("border-start");
        }
        else {
            questionItem.classList.remove("border-warning")
            questionItem.classList.add("border")
            questionItem.classList.remove("border-start");
        }
    }  
 
    function createQuestion() {
        const clone = questionTemplate.content.cloneNode(true);
        const questionElement = clone.querySelector(".question-item");
        questionElement.innerHTML = questionElement.innerHTML.replace(/{{index}}/g, questionCount);
        questionElement.dataset.id = questionCount;
        ChangeQuestionType(questionElement, questionCount++);
        questionContainer.appendChild(questionElement);
    }
    //questionElement.addEventListener('click', (e) => {
    //    if (e.target.classList.contains('question-item') || e.target.classList.contains('check-wrap')) {
    //        const checkBox = questionElement.querySelector(".check-wrap input");
    //        checkBox.checked = !checkBox.checked;
    //        checkBox.dispatchEvent(new Event('change'));
    //    }
    //});

    //questionElement.querySelector(".check-wrap input").addEventListener("change", (e) => {
    //    if (e.target.checked) {
    //        questionElement.classList.add("border-warning");
    //        questionElement.classList.remove("border")
    //        questionElement.classList.add("border-start");
    //    }
    //    else {
    //        questionElement.classList.remove("border-warning")
    //        questionElement.classList.add("border")
    //        questionElement.classList.remove("border-start");
    //    }
    //});
    //questionElement.querySelector(".description-check").addEventListener('change', () => {
    //    questionElement.querySelector(".description-container").classList.toggle("d-none");
    //});
    function reindexOptions(container) {
        container.querySelectorAll(`.option-container`).forEach((container, index) => {
            container.querySelectorAll(".option-input").forEach(input => {
                input.name = input.name.replace(/AnswerOptions\[\d+\]/g, `AnswerOptions[${index}]`);
            })           
        });
    }
    //[name *= "AnswerOptions["]
    function createCheckBox(questionIndex, optIndex) {
        const checkBoxContainer = document.createElement("div");
        checkBoxContainer.className = "d-flex align-items-center gap-2  p-2 rounded option-container";

        const checkTextInput = document.createElement("input");
        checkTextInput.type = "text";
        checkTextInput.className = `form-control option-input`;
        checkTextInput.placeholder = "Option text";
        checkTextInput.name = `Questions[${questionIndex}].AnswerOptions[${optIndex}].Text`;

        const checkInput = document.createElement("input");
        checkInput.type = "checkbox";
        checkInput.disabled = true;
        checkInput.className = "form-check-input me-2";

        const closeBtn = document.createElement("button");
        closeBtn.className = "btn btn-sm btn-outline-danger ms-auto close-option";
        closeBtn.innerHTML = `<i class="bi bi-x"></i>`;        

        checkBoxContainer.appendChild(checkInput);
        checkBoxContainer.appendChild(checkTextInput);
        checkBoxContainer.appendChild(closeBtn);
        return checkBoxContainer;
    }
    //closeBtn.addEventListener('click', (e) => {
    //    e.preventDefault();
    //    const cntr = e.target.closest(".checkboxes-container");
    //    e.target.closest('div').remove();
    //    reindexOptions(cntr);
    //});

    function createAnswerElement(type, questionIndex) {
        const container = document.createElement("div");
        switch (type) {
            case "1":
                container.innerHTML = `
                      <label class="form-label fw-semibold">Answer</label>
                      <textarea class="form-control" rows="3" disabled></textarea>`;
                break;
            case "2":
                container.innerHTML = `
                      <label class="form-label fw-semibold">Options</label>
                      <div class="checkboxes-container mb-2"></div>
                      <button type="button" class="btn btn-sm btn-outline-primary add-option">
                         <i class="bi bi-plus-circle"></i> Add Option
                      </button> `;
                const checkboxesContainer = container.querySelector('.checkboxes-container');
                checkboxesContainer.appendChild(createCheckBox(questionIndex, 0));               
                break;
            case "3":
                container.innerHTML = `
                          <label class="form-label fw-semibold">Answer</label>
                          <input type="number" class="form-control" disabled>`;
                break;
            default:
                container.innerHTML = `
                         <label class="form-label fw-semibold">Answer</label>
                         <input type="text" class="form-control" disabled>`;
        }
        return container;
    }
    //container.querySelector('.add-option').addEventListener('click', (e) => {
    //    e.preventDefault();
    //    const optionIndex = checkboxesContainer.querySelectorAll("div").length;
    //    checkboxesContainer.appendChild(createCheckBox(questionIndex, optionIndex));
    //});
});