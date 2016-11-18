var mask = document.getElementById("left-nav-mask");
var content = document.getElementById("left-nav-content");
var maintoggler = document.getElementById("main-nav-toggler");
var lefttoggler = document.getElementById("left-nav-toggler");
var container = document.getElementById("main-container");
var header = document.getElementById("blog-header");
var footer = document.getElementById("blog-footer");
var isOpen = false;

resizeHandler();
window.onresize = resizeHandler;

maintoggler.onclick = togglerClickHandler;
lefttoggler.onclick = togglerClickHandler;
mask.onclick = togglerClickHandler;

function togglerClickHandler() {
    if (isOpen === true) {
        content.setAttribute("class", "left-nav-content left-nav-content-hide");
        content.style.transform = "translate(-266px, 0px)";
        mask.style.display = "none";
    } else {
        content.setAttribute("class", "left-nav-content left-nav-content-show");
        content.style.transform = "translate(0px, 0px)";
        mask.style.display = "block";
    }
    isOpen = !isOpen;
};

function resizeHandler() {
    mask.style.height = window.innerHeight + 'px';
    mask.style.width = window.innerWidth + 'px';
    content.style.height = window.innerHeight + 'px';

    var minheight = window.innerHeight - header.offsetHeight - footer.offsetHeight;
    container.setAttribute("style", "min-height:" + minheight + 'px');
}