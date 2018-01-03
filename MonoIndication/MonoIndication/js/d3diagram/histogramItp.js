
var svg = d3.select("#chart").append("svg");
margin = { top: 80, right: 20, bottom: 40, left: 60 };
var width, height;

// функция пересчитывает размеры svg элемента (при ресайзе)
function updateDimensions(winWidth) {
    width = winWidth - margin.left - margin.right;
    height = 450 - margin.top - margin.bottom;
}


// Prep the tooltip bits, initial display is hidden


var x = d3.scale.ordinal(), 
    y = d3.scale.linear();


var x_axis = d3.svg.axis().scale(x).orient("bottom"),
    y_axis = d3.svg.axis().scale(y).orient("left");



var g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

g.append("g").attr("class", "axis x")
    .append("text")
    .style("font-weight", "bold");

g.append("g").attr("class", "axis y")
    .append("text")
    .attr("transform", "rotate(-90)")
    .attr("x", 0)
    .attr("dy", ".71em")
    .style("text-anchor", "end")
    .style("font-weight", "bold");

var info = g.append("g")
   .append("text")
   .attr("font-size", 30)
   .attr("fill", "#f8805e")
   .attr("text-anchor", "end");



function drawHistogramm(data) {

    x.domain(data.map(function (d) { return moment(d.month, "M").format("MMM"); }));
    y.domain([0, d3.max(data, function (d) { return d.value; })]);

   
    redrawHistogramm();


    var bars = g.selectAll(".bar").data(data);
    
    var labels = g.selectAll(".txt-label").data(data);
       

    bars.enter().append("rect").attr("class", "bar").attr("x", function(d) { return x(moment(d.month, "M").format("MMM")); })
        .attr("y", function(d) { return y(d.value); })
        .attr("width", x.rangeBand())
        .attr("height", function(d) { return height - y(d.value); })
        .on("mouseover", function (d) {
            info.text("c: " + d.startperiod + " по: "+d.endperiod);
        })
        .on("mouseout", function () { info.text(""); });

    bars.attr("y", function(d) { return y(d.value); })
        .attr("height", function(d) { return height - y(d.value); });

    labels.enter().append("text").attr("class","txt-label")
        .attr("fill", "#cd3827")
        .attr("font-size", "1.1vw")
        .attr("x", function (d) { return x(moment(d.month, "M").format("MMM")); })
        .attr("y", function (d) { return y(d.value); })
        .text(function (d) { return d.value.toFixed(1); });    


    labels.attr("y", function (d) { return y(d.value); })
          .text(function (d) { return d.value.toFixed(1); });;


}

function redrawHistogramm() {
    
    updateDimensions($("#chart").width());

    svg.attr("width", width+margin.left+margin.right).attr("height",height+margin.top+margin.bottom);
    

    x.rangeBands([0, width], .2);
    y.rangeRound([height,0]);
    
    g.select(".axis.x")
       .attr("transform", "translate(0," + height + ")")
       .call(x_axis)
       .select("text")
       .attr("x", x("июнь"))
       .attr("y", margin.bottom)
       .text("Месяц года");

    g.select(".axis.y")
        .call(y_axis)
        .select("text")
        .attr("y", 5)
        .text("Потребление, Гкал");
    
   
    g.selectAll(".bar")
        .attr("x", function(d) { return x(moment(d.month, "M").format("MMM")); })
        .attr("width", x.rangeBand());
    
    g.selectAll(".txt-label")
        .attr("x", function (d) { return x(moment(d.month, "M").format("MMM")); });

    info.attr("x", width)
        .attr("y", 0 - (margin.top / 2));
}