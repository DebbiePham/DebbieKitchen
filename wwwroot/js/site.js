// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// const { check } = require("prettier");

// Write your JavaScript code.

function setThemeFromStorage(){
    const theme = localStorage.getItem("theme");
    if(theme != null){
        document.getElementById("mode-theme").setAttribute("data-theme", theme);
    } 
    
    console.log("saving theme loading....");
}


// set theme
function setTheme(checkedbox){
    
    console.log(checkedbox);
    if(checkedbox.checked){
        console.log("the box has been checked");
        document.getElementById("mode-theme").setAttribute("data-theme", "dark");
        localStorage.setItem("theme", "dark");
    } else {
        console.log("the box got unchecked");
        document.getElementById("mode-theme").setAttribute("data-theme", "cupcake");
        localStorage.setItem("theme", "cupcake");
    }
}

setThemeFromStorage();

function recipePrint(){
    var btn = document.getElementById("print-btn");
        btn.onclick = () =>{
            window.print();
        }
}