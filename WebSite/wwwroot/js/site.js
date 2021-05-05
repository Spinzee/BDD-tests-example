// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    //const buttons = $('.product-button');

    //buttons.click(handleClick);

    $(".product-buttons .product-button").on("click", handleClick);

    $(".product-buttons .product-button").hover(function() {
        $(this).toggleClass("light-highlight");
    });

    function handleClick(event) {
        const buttonClicked = event.target;
        const serviceClicked = buttonClicked.dataset["serviceType"];
        
        $(".product-button").each(function () {
            const buttonServiceType = this.dataset["serviceType"];

            if (buttonServiceType === serviceClicked) {
                if (!$(buttonClicked).hasClass("active")) {
                    $(this).removeClass("active");
                }
            }
        });

        $(buttonClicked).toggleClass("active");

        $.getJSON("Products.json", { "Name" : "Wet Cut" }),
            function(data) {
                alert(data.Name);
            }

        //$("#stylist-ajax").load("Stylist.html #stylists",
        //    function(response, status, xhr) {
        //        if (status == "error") {
        //            alert('Error: ' + xhr.statusText);
        //        }
        //    });
    }


    //let buttons = Array.from(document.querySelectorAll(".product-button"));

    //buttons.forEach(button => button.addEventListener('click', handleButtonClick));

    //function handleButtonClick(e) {

    //    const service = e.target.getAttribute('data-service-type');

    //    if (e.currentTarget.classList.contains("active")) {
    //        e.currentTarget.classList.remove("active");
    //        return;
    //    }

    //    for (let i = 0; i < buttons.length; i++) {
    //        if (buttons[i].dataset['serviceType'] === service) {
    //            buttons[i].classList.remove("active");
    //        }
    //    }

    //    e.currentTarget.classList.add("active");

    //}

    //this is jquery

    //const button = document.querySelector(".product-button");

    //button.onclick = function (event) {

    //    console.log("Button clicked." + event.currentTarget);
    //};
});