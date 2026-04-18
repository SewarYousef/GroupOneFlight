/* ===========================
   Client-Side Date Validation
   =========================== */

/**
 * Validates flight date input
 * Rules:
 * - Date must be greater than today
 * - Date must be <= today + 3 years
 */

(function () {
    'use strict';

    // Get all date input fields with data-validate-date attribute
    const dateInputs = document.querySelectorAll('input[type="date"][data-validate-date]');

    dateInputs.forEach(function (input) {
        // Real-time validation on change
        input.addEventListener('change', function () {
            validateDate(this);
        });

        // Validation on blur
        input.addEventListener('blur', function () {
            validateDate(this);
        });
    });

    /**
     * Validates a date input field
     * @param {HTMLInputElement} dateInput - The date input element
     */
    function validateDate(dateInput) {
        const selectedDate = new Date(dateInput.value);
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        const maxDate = new Date(today);
        maxDate.setFullYear(maxDate.getFullYear() + 3);

        const errorElement = dateInput.nextElementSibling;
        const isValid = validateDateRange(selectedDate, today, maxDate);

        if (!isValid && dateInput.value) {
            // Show error message
            const errorMsg = `Date must be after ${formatDate(today)} and within 3 years (by ${formatDate(maxDate)}).`;
            displayError(dateInput, errorMsg);
        } else {
            // Clear error message
            clearError(dateInput);
        }
    }

    /**
     * Checks if date is within valid range
     * @param {Date} selectedDate - The selected date
     * @param {Date} today - Today's date
     * @param {Date} maxDate - Maximum allowed date (today + 3 years)
     * @returns {boolean} - True if date is valid
     */
    function validateDateRange(selectedDate, today, maxDate) {
        return selectedDate > today && selectedDate <= maxDate;
    }

    /**
     * Formats date for display
     * @param {Date} date - The date to format
     * @returns {string} - Formatted date string (YYYY-MM-DD)
     */
    function formatDate(date) {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    /**
     * Displays an error message below the input
     * @param {HTMLInputElement} input - The input element
     * @param {string} message - The error message
     */
    function displayError(input, message) {
        // Remove existing error if present
        clearError(input);

        // Create error element
        const errorDiv = document.createElement('div');
        errorDiv.className = 'invalid-feedback d-block';
        errorDiv.textContent = message;

        // Add class to input for styling
        input.classList.add('is-invalid');

        // Insert error message after input
        input.parentNode.insertBefore(errorDiv, input.nextSibling);
    }

    /**
     * Clears error message for an input
     * @param {HTMLInputElement} input - The input element
     */
    function clearError(input) {
        input.classList.remove('is-invalid');

        // Remove error message element if exists
        const errorDiv = input.nextElementSibling;
        if (errorDiv && errorDiv.classList.contains('invalid-feedback')) {
            errorDiv.remove();
        }
    }

    /**
     * Validates date before form submission
     * Attach to form's onsubmit event
     * @param {Event} event - The form submit event
     * @returns {boolean} - True if all dates are valid
     */
    window.validateAllDates = function (event) {
        let allValid = true;

        dateInputs.forEach(function (input) {
            if (input.value) {
                const selectedDate = new Date(input.value);
                const today = new Date();
                today.setHours(0, 0, 0, 0);
                const maxDate = new Date(today);
                maxDate.setFullYear(maxDate.getFullYear() + 3);

                if (!validateDateRange(selectedDate, today, maxDate)) {
                    allValid = false;
                    const errorMsg = `Date must be after ${formatDate(today)} and within 3 years (by ${formatDate(maxDate)}).`;
                    displayError(input, errorMsg);
                }
            }
        });

        return allValid;
    };

})();
