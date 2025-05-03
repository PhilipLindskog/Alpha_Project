document.addEventListener("DOMContentLoaded", () => {
    const dropdownButtons = document.querySelectorAll('.project.card .btn-action');

    dropdownButtons.forEach((btn, index) => {
        const dropdown = btn.parentElement.querySelector('.dropdown');

        if (dropdown) {
            // Skapa ett unikt ID
            const uniqueId = `project-dropdown-${index + 1}`;
            dropdown.id = uniqueId;
            btn.setAttribute('data-target', `#${uniqueId}`);
        }
    });
});
// Delen över är genererad med ChatGPT 4o för att fixa till dropdowns för projecten

document.querySelectorAll('.upload-trigger').forEach(trigger => {
    const container = trigger.closest('.form-group');
    const fileInput = container.querySelector('.image-upload');
    const previewImage = container.querySelector('.image-preview');
    const icon = container.querySelector('.image-preview-icon');
    const iconContainer = container.querySelector('.image-preview-icon-container');

    if (!fileInput || !previewImage) return;

    trigger.addEventListener('click', () => {
        fileInput.click();
    });

    fileInput.addEventListener('change', (e) => {
        const file = e.target.files[0];
        if (file && file.type.startsWith('image/')) {
            const reader = new FileReader();

            reader.onload = (e) => {
                previewImage.src = e.target.result;
                previewImage.classList.remove('hide');
                iconContainer.classList.add('selected');
                icon.classList.remove('fa-camera');
                icon.classList.add('fa-pen-to-square');
            };

            reader.readAsDataURL(file);
        }
    });
});
// Delen över är tillfixad med ChatGPT 4o för att gör så att alla modaler kan ladda up filer.


const dropdowns = document.querySelectorAll('[data-type="dropdown"]')

document.addEventListener('click', function (event) {
    let clickedDropdown = null

    dropdowns.forEach(dropdown => {
        const targetId = dropdown.getAttribute('data-target')
        const targetElement = document.querySelector(targetId)

        if (dropdown.contains(event.target)) {
            clickedDropdown = targetElement

            document.querySelectorAll('.dropdown.dropdown-show').forEach(openDropdown => {
                if (openDropdown !== targetElement) {
                    openDropdown.classList.remove('dropdown-show')
                }
            })

            targetElement.classList.toggle('dropdown-show')
        }
    })

    if (!clickedDropdown && !event.target.closest('.dropdown')) {
        document.querySelectorAll('.dropdown.dropdown-show').forEach(openDropdown => {
            openDropdown.classList.remove('dropdown-show')
        })
    }
})

const modals = document.querySelectorAll('[data-type="modal"]')
modals.forEach(modal => {
    modal.addEventListener('click', function () {
        const targetId = modal.getAttribute('data-target')
        const targetElement = document.querySelector(targetId)

        targetElement.classList.add('modal-show')
    })
})

const closeButtons = document.querySelectorAll('[data-type="close"]')
closeButtons.forEach(button => {
    button.addEventListener('click', function () {
        const targetId = button.getAttribute('data-target')
        const targetElement = document.querySelector(targetId)

        targetElement.classList.remove('modal-show')
    })
})

document.querySelectorAll('.form-select').forEach(select => {
    const trigger = select.querySelector('.form-select-trigger')
    const triggerText = trigger.querySelector('.form-select-text')
    const options = select.querySelectorAll('.form-select-option')
    const hiddenInput = select.querySelector('input[type="hidden"]')
    const placeholder = select.dataset.placeholder || "Choose"

    const setValue = (value = "", text = placeholder) => {
        triggerText.textContent = text
        hiddenInput.value = value
        select.classList.toggle('has-placeholder', !value)
    };

    setValue();

    trigger.addEventListener('click', (e) => {
        e.stopPropagation();
        document.querySelectorAll('form-select.open')
            .forEach(el => el !== select && el.classList.remove('open'))
        select.classList.toggle('open')
    })

    options.forEach(option =>
        option.addEventListener('click', () => {
            setValue(option.dataset.value, option.textContent)
            select.classList.remove('open')
        })
    )

    document.addEventListener('click', e => {
        if (!select.contains(e.target))
            select.classList.remove('open')
    })
})

// handle submit forms
const forms = document.querySelectorAll('form')
forms.forEach(form => {
    form.addEventListener('submit', async (e) => {
        e.preventDefault()

        clearErrorMessages(form)

        const formData = new FormData(form)

        try {
            const res = await fetch(form.action, {
                method: 'post',
                body: formData
            })

            if (res.ok) {
                const modal = form.closest('.modal')
                if (modal)
                    modal.style.display = 'none';

                window.location.reload()
            }
            else if (res.status === 400) {
                const data = await res.json()

                if (data.errors) {
                    Object.keys(data.errors).forEach(key => {
                        const input = form.querySelector(`[name="${key}"]`)
                        if (input) {
                            input.classList.add('input-validation-error')
                        }

                        const span = form.querySelector(`[data-valmsg-for="${key}"]`)
                        if (span) {
                            span.innerText = data.errors[key].join('\n')
                            span.classList.add('field-validation-error')
                        }
                    })
                }
            }
        }
        catch {
            console.log('error submitting the form')
        }
    })
})

function clearErrorMessages(form) {
    form.querySelectorAll('[data-val="true"]').forEach(input => {
        input.classList.remove('input-validation-error')
    })

    form.querySelectorAll('[data-valmsg-for]').forEach(span => {
        span.innerText = ''
        span.classList.remove('field-validation-error')
    })
}
