Singular.sequentialPromise = (generator) => {
  let iterator = generator(); // create generator object
  recursivelyNext();

  // this functions keeps calling next() if a promise is yielded
  function recursivelyNext(data) {
    let yielded = iterator.next.apply(iterator, arguments); // { value: Any, done: Boolean }

    if (isPromise(yielded.value)) {
      yielded.value.then((data) => {
        recursivelyNext(data);
    });
  }

  function isPromise(val) {
    return val && typeof val.then === 'function';
  }
}
}