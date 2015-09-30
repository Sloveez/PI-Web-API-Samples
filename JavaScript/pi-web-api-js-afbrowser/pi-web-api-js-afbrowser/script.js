// Define the names and object types of the AF hierarchy
var childrenMap = {
    PISystems: ['AssetServers'],
    AssetServers: ['Databases'],
    Databases: ['Elements'],
    Elements: ['Elements', 'Attributes'],
    Attributes: ['Attributes']
};

// Constructor for the node object
function node(name, type, links, parentDiv) {
    this.type = type;
    this.links = links;
    this.flipper = $('<span class="flipper">+</span>').click(flip.bind(this, this));
    parentDiv.append(this.flipper).append('<span class="' + type + '"> ' + name + '</span><br />');
    this.div = $('<div></div>').hide().appendTo(parentDiv);
}

// Make ajax calls to PI Web API server to get the children of each node
function loadChildren(n) {
    n.loaded = true;
    childrenMap[n.type].forEach(function (childCollection) {
        $.ajax({
            url: n.links[childCollection],
            success: function (collection) {
                n[childCollection] = collection.Items.map(function (item) {
                    return new node(item.Name, childCollection, item.Links, n.div);
                })
            },
            error: function (xhr) {
                console.log(xhr.responseText);
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Basic xxx');
            }
        })
    });
}

// Load children, toggle the flipper icon and show/hide the loaded element
function flip(n) {
    if (!n.loaded) { loadChildren(n); }
    n.flipper.html(n.flipper.html() == '+' ? '-' : '+');
    n.div.toggle();
}

// Add the root node when the DOM is fully loaded
$(function () {
    root = new node('PI Systems', 'PISystems',
      { AssetServers: 'https://myserver/piwebapi/assetservers' }, $("#root"));
});