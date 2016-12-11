function plotData(count, p, value, index, closestY)
  %plotting
  
  clf;
  x = [1:length(count)];
  %all words as points
  plot (x, p, "o", "markersize", 4);
  hold;
  
  %current point
  plot(x(index), p(index),'LineWidth', 2, "gx", "markersize", 15)
  
  %fill answered points with red
  anweredPoints = find(p == 0 | p == 1);
  sx = x(anweredPoints);
  sy = p(anweredPoints)';
  plot(sx, sy,'LineWidth',4, "r*", "markersize", 2);

  %running average plot
  windowSize = 5;
  if length(sy) >= windowSize
    yy = runningAverage(sy, sy, windowSize);
    plot(sx,yy, 'linewidth', 2, '--');
    
    %fitting polynomial
    polynY = fitPoly(length(p), sx, sy, min(windowSize, length(x)));
    plot(sx, polynY, ":k",'LineWidth', 2)
    
    legend ("normailzed count", "current question", "answered", "running average", "fitted polynomial");
  else
    legend ("normailzed count", "current question", "answered");
  end
  
  cy = ones(1,length(x)) * closestY;
  plot(x, cy, 'linewidth', 2, '--'); 
endfunction