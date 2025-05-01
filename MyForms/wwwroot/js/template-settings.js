document.addEventListener("DOMContentLoaded", () => {
    const addTagBtn = document.getElementById("add-tag");
    const tagInput = document.getElementById("tag-input");
    const tagContainer = document.getElementById("tag-container");
    const isPrivateCheckBox = document.getElementById("is-private-check");
    const emailSection = document.getElementById("add-email");
    const emailInput = document.getElementById("email-input");
    const templateId = document.getElementById("template-id").value; 
    const addEmailBtn = document.getElementById("add-email-btn");
    const emailContainer = document.getElementById("email-container");
    const emailErrorMsg = document.getElementById('email-error');
    function createBadge(inputValue, inputName, container) {
        const tagWrap = document.createElement("div");

        const input = document.createElement("input");
        input.value = inputValue;
        input.hidden = true;
        input.name = inputName;

        const tagBadge = document.createElement("span");
        tagBadge.classList.add("badge", "rounded-pill", "bg-secondary");
        tagBadge.innerHTML = `${inputValue} <button type="button" class="btn-close btn-close-white ms-1"></button>`;

        tagWrap.appendChild(tagBadge);
        tagWrap.appendChild(input);
        container.appendChild(tagWrap);
    }
    tagContainer.addEventListener('click', (e) => {
        if (e.target.nodeName == "BUTTON") {
            e.target.closest("div").remove();
        }
    });
    emailContainer.addEventListener('click', (e) => {
        if (e.target.nodeName == "BUTTON") {
            e.target.closest("div").remove();
        }
    });

    addTagBtn.addEventListener('click', (e) => {
        e.preventDefault();
        const tagInputValue = tagInput.value.trim();
        if (tagInputValue.length < 1 || isDuplicate("#tag-container input", tagInputValue)) {
            return;
        }
        createBadge(tagInputValue, "tagNames", tagContainer);
        tagInput.value = "";
    })

    isPrivateCheckBox.addEventListener('change', () => {
        emailSection.hidden = !isPrivateCheckBox.checked;
    });


    addEmailBtn.addEventListener('click', async (e) => {
        e.preventDefault();
        let emailValue = emailInput.value.trim();
        if (emailValue.length < 1 || isDuplicate("#email-container input", emailValue)) {
            return;
        }
        let result= await checkEmail(emailValue)
        if (!result.isExist || result.isAllowed) {
            emailErrorMsg.classList.remove("d-none");
            emailErrorMsg.innerText = result.message;
            return;
        }
        emailErrorMsg.classList.add("d-none");

        createBadge(emailValue, "emails", emailContainer);
        emailInput.value = "";
    })

    emailInput.addEventListener('keypress', (e) => {
        if (e.key == "Enter") {
            e.preventDefault();
            addEmailBtn.click();
        }
    });

    function isDuplicate(selector, value) {
        return [...document.querySelectorAll(selector)].some(x => x.value.toUpperCase() == value.toUpperCase());        
    }
    async function checkEmail(email) {
        const response = await fetch(`/checkEmail?email=${email}&templateId=${templateId}`);
        const data = await response.json();
        return data;
    }
    function enableTypeHead(inputId, url) {
        $(`#${inputId}`).typeahead({
            minLength: 1, 
            highlight: true, 
            source: function (query, process) {
                return $.get(url, { query: query }, function (data) {
                    return process(data); 
                });
            }
        });
    }
    enableTypeHead('tag-input', "/tags");
    enableTypeHead('email-input', "/emails");
})