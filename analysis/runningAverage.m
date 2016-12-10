function yy = runningAverage(x, y, windowSize)
    if length(x) >= windowSize
      n = windowSize;
      m = (n - 1) / 2;
      len = size(x, 2);
      yy=x;
      div = ones(size(x));
      for k = 1:m
         k2 = 2*k;   % Slightly faster
         z  = zeros(1, k);
         yy  = yy + [z, x(1:len - k2) + x(1 + k2:len), z];
         div(k + 1:len - k) = div(k + 1:len - k) + 2;
      end
      yy = yy ./ div;
    else
      yy = y;  
    end  
endfunction