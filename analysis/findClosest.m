function [c index] = findClosest(vector, value)
  [c index] = min(abs(vector - value));
endfunction   