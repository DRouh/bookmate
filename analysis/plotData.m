function plotData(count, p, value, index, closestY)
  %plotting
  clf;
  subplot(2,1,1);
  
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

  %plot where find our word
  cy = ones(1,length(x)) * closestY;
  plot(x, cy, 'linewidth', 2, '--'); 
  
  title('Vocabulary estimate');
  xlabel('Indices of ranked words') % x-axis label
  ylabel('Probability of knowing') % y-axis label
  legend ("normailzed count", "current question", "answered", "search area");
  %running average plot
  
  windowSize = 5;
  if length(sy) >= windowSize
    subplot(2,1,2);
    yy = runningAverage(sy, sy, windowSize);
    plot(sx,yy, 'linewidth', 2, '--');
    hold;
    %fitting polynomial
    deg = min(windowSize, length(x));
    polynY = fitPoly(length(p), sx, sy, deg);
    plot(sx, polynY, ":k",'LineWidth', 2)
    title('Vocabulary estimate');
    xlabel('Indices of ranked words') % x-axis label
    ylabel('Probability of knowing') % y-axis label
    legend ("running average", "fitted polynomial");
  end

endfunction