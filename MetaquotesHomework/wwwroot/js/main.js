/*jshint esversion: 6 */
const limit = 10;
const ipRegex = /(\d{1,3}\.){3}\d{1,3}/i;

function renderFindIPView() {
    const template = document.getElementById("findTemplate");
    const renderFn = Handlebars.compile(template.innerHTML);
    return renderFn({"btn_title": "Find IP address", "placeholder": "IP address", "data_attr": "data-find-ip"});
}

function renderFindCityView() {
    const template = document.getElementById("findTemplate");
    const renderFn = Handlebars.compile(template.innerHTML);
    return renderFn({"btn_title": "Find city", "placeholder": "City", "data_attr": "data-find-city"});
}

function findParameter(parameterName) {
    const searchParams = new URLSearchParams(window.location.search);
    return searchParams.get(parameterName);
}

function findCityfromLocation() {
    let q = findParameter("q");
    if(q == undefined || q.length == 0)
        return;
    document.getElementById('q').value = q;
    findCity(q, 0);
}

function findIPfromLocation() {
    let q = findParameter("q");
    if(q == undefined || q.length == 0)
        return;
    document.getElementById('q').value = q;
    findIP(q);
}

function findCity(q, offset, append) {
    if(q == undefined || q.length == 0)
        return;
    find(`/city/locations?city=${encodeURIComponent(q)}&offset=${offset}&limit=${limit}`, append);
}


function findIP(q) {
    if(q == undefined || q.length == 0)
        return;
    find('/ip/location?ip='+ encodeURIComponent(q));
}

function find(url, append) {
    fetch(url, { Method: 'GET' })
    .then(response => {
        if(response.status == 200) {
            return new Promise(resolve => response.json()
                .then(json => resolve({
                    status: response.status,
                    json,
                })));
        }
        return new Promise(resolve => resolve({
            status: response.status,
            json: {},
        }));
    })
    .then(({ status, json }) => {
        if(status == 200) {
            const template = document.getElementById("resultTemplate");
            const renderFn = Handlebars.compile(template.innerHTML);
            if(!Array.isArray(json)) {
                json = [json];
            }
            if(append) {
                let content = document.createElement("div");
                content.innerHTML = renderFn(json);
                document.getElementById('result').appendChild(content);
            }
            else {
                document.getElementById('result').innerHTML= renderFn(json);
            }
            if(json.length == limit) {
                let offset = findParameter('offset');
                if(offset == undefined || offset == null || offset.length == 0) {
                    offset = "0";
                }
                offset = parseInt(offset) + limit;
                let btn = document.getElementById('find-more');
                btn.setAttribute("data-offset", offset.toString());
                btn.classList.remove("hidden");
            } else {
                document.getElementById('find-more').classList.add("hidden");
            }
        } else if(status == 400) {
            const template = document.getElementById("badRequestTemplate");
            const renderFn = Handlebars.compile(template.innerHTML);
            document.getElementById('result').innerHTML= renderFn({});
        } else if(status == 404) {
            const template = document.getElementById("notFoundTemplate");
            const renderFn = Handlebars.compile(template.innerHTML);
            document.getElementById('result').innerHTML= renderFn({});
        } else {
            const template = document.getElementById("errorTemplate");
            const renderFn = Handlebars.compile(template.innerHTML);
            document.getElementById('result').innerHTML= renderFn({});
        } 
    })
    .catch(error => {
          console.error('Fetch failed:', error);
          const template = document.getElementById("errorTemplate");
          const renderFn = Handlebars.compile(template.innerHTML);
          document.getElementById('result').innerHTML= renderFn({});
    });
}

const routes = {
    "/": { title: "Home", render: renderFindIPView, processor: findIPfromLocation },
    "/app/ip": { title: "Find IP", render: renderFindIPView, processor: findIPfromLocation },
    "/app/city": { title: "Find city", render: renderFindCityView, processor: findCityfromLocation },
};

function router() {
    let view = routes[location.pathname];
    if (view) {
        document.title = view.title;
        app.innerHTML = view.render();
        view.processor();
    } else {
        history.replaceState("", "", "/");
        router();
    }
}

function onSubmit(e) {
    if (e.target.matches("[data-find-more]")) {
        const val = document.getElementById('q').value;
        const attr = document.getElementById('find-more').getAttribute('data-offset');
        var offset = "0";
        if(attr != undefined && attr != null) {
            offset = attr;
        }
        findCity(val, offset, true);
    } else if (e.target.matches("[data-find-ip]")) {
        const val = document.getElementById('q').value;
        if(val == undefined || val == null || val.length == 0)
            return;
        if(!ipRegex.test(val)) {
            alert("IP must match pattern XXX.XXX.XXX.XXX");
        }
        e.preventDefault();
        history.pushState("", "", "/app/ip?q=" + encodeURIComponent(val));
        findIP(val);
    } else if (e.target.matches("[data-find-city]")) {
        const val = document.getElementById('q').value;
        if(val == undefined || val == null || val.length == 0)
            return;
        e.preventDefault();
        history.pushState("", "", "/app/city?q=" + encodeURIComponent(val));
        findCity(val, 0);
    } else if (e.target.matches("[data-link]")) {
        e.preventDefault();
        history.pushState("", "", e.target.href);
        router();
    }
}

window.addEventListener("popstate", router);
window.addEventListener("DOMContentLoaded", router);
window.addEventListener("click", function (e) {
    if (e.target.type === 'submit')
        onSubmit(e);
});
window.addEventListener('keypress', function (e) {
    if (e.key === 'Enter' && 
        (e.target.matches("[data-find-city]") || e.target.matches("[data-find-ip]")))
        onSubmit(e);
});

