import React from 'react';
import { render } from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { Provider } from 'react-redux';
import { applyMiddleware, combineReducers, compose, createStore } from 'redux';
import { createLogger } from 'redux-logger';
import createSagaMiddleware from 'redux-saga';
import App from './app';
import * as reducers from './store/reducers';
import rootSaga from './store/sagas';
import './styles/main.scss';

const sagaMiddleware = createSagaMiddleware();

const store = createStore(
  combineReducers(reducers as any),
  compose(applyMiddleware(createLogger(), sagaMiddleware))
);

sagaMiddleware.run(rootSaga);

render(
  <AppContainer>
    <Provider store={store}>
      <App />
    </Provider>
  </AppContainer>,
  document.getElementById('root')
);
