// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



// Каталог товарів (TreeView)
var toggler = document.getElementsByClassName("TreeViewCaret");
var i;

for (i = 0; i < toggler.length; i++) {
    toggler[i].addEventListener("click", function () {
        this.parentElement.querySelector(".TreeViewNested").classList.toggle("TreeViewActive");
        this.classList.toggle("TreeViewCaret-down");
    });
}
