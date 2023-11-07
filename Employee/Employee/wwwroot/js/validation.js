function validateDateOfBirth() {
    var dobField = $("#DateOfBirth");
    dobField.on("change", function () {
        var dob = new Date(dobField.val());
        var now = new Date();
        var age = now.getFullYear() - dob.getFullYear();

        if (age < 18) {
            alert("Employee must be at least 18 years old.");
            dobField.val("");
        } else if (dob > now) {
            alert("Date of Birth cannot be in the future.");
            dobField.val("");
        }
    });
}

function validateDateOfJoining() {
    var dobField = $("#DateOfBirth");
    var dojField = $("#DateOfJoining");
    dobField.on("change", function () {
        var dob = new Date(dobField.val());
        var doj = new Date(dojField.val());
        var ageAtJoining = doj.getFullYear() - dob.getFullYear();
        var now = new Date();

        
        if (doj < dob) {
            alert("Date of Joining cannot be before Date of Birth.");
            dojField.val("");
        } else if (ageAtJoining < 18) {
            alert("Employee must be at least 18 years old at the time of joining.");
            dojField.val("");
        }
    });

    dojField.on("change", function () {
        var dob = new Date(dobField.val());
        var doj = new Date(dojField.val());
        var ageAtJoining = doj.getFullYear() - dob.getFullYear();
        var now = new Date();


        if (doj < dob) {
            alert("Date of Joining cannot be before Date of Birth.");
            dojField.val("");
        } else if (ageAtJoining < 18) {
            alert("Employee must be at least 18 years old at the time of joining.");
            dojField.val("");
        }
    });
}

        

function validateEmail() {
    var emailField = $("#Email");
    emailField.on("blur", function () {
        var email = emailField.val();
        var emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;

        if (!emailRegex.test(email)) {
            alert("Invalid email format.");
            emailField.val("");
        }
    });
}

$(document).ready(function () {
    validateDateOfBirth();
    validateDateOfJoining();
    validateEmail();
});
