function plotData(count, p, value, index)
  clf;

  %plotting
  x = [1:length(count)];
  
  %words
  plot (x, p, "o", "markersize", 7);
  hold;
  
  %current point
  plot(x(index), p(index),'LineWidth',2, "gx", "markersize", 15)

  anweredPoints = find(p == 0 | p == 1);

  if length(anweredPoints) >= 2
    sx = x(anweredPoints);
    sy = p(anweredPoints)';
    plot(sx, sy,'LineWidth',2, "r*", "markersize", 3);
    %yy = spline(sx, sy, x);
    %yy
    %max(yy)
    %mm =  abs(max(yy));
    %yz = abs(yy/mm);
    %max(yz)
    %plot(x,yz);
    %plot(x, yy,'--','LineWidth', 1);

    windowSize = 10;
    b = ones(1,windowSize)/windowSize;
    yy = filter(b, 1, sy);
    plot(sx,yy);
    legend ("normailzed count", "current question", "answered", "probability locus");
  else
    legend ("normailzed count", "current question", "answered");
  end
endfunction