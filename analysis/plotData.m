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
  leg = '';
  if length(sy) >= windowSize 
    subplot(2, 1, 2);
    yy = runningAverage(sy, sy, windowSize);
    plot(sx,yy, 'linewidth', 2, '--');
    hold;
    
    raLeg=true;
  end
  
  if length(sx) >= 2  
    subplot(2, 1, 2);
    %fitting polynomial
    deg = 2;
    polynY = fitPoly(length(p), sx, sy, deg);
    plot(sx, polynY, ":k",'LineWidth', 2);
    
    fpLeg = true;

  end
  if length(sy) >= windowSize  || length(sx) >= 2 
    title('Vocabulary estimate');
    xlabel('Indices of ranked words');
    ylabel('Probability of knowing');
    
    if exist('fpLeg') && exist('raLeg')
      legend('running average', 'fitted polynomial');
    elseif exist('fpLeg')
      legend( 'fitted polynomial');
    elseif exist('fpLeg')
      legend('running average');
    end   
  end  
endfunction