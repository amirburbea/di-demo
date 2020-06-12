import {
  Alignment,
  Button,
  Classes,
  Navbar,
  NavbarGroup,
} from '@blueprintjs/core';
import classNames from 'classnames';
import React, { FunctionComponent, useCallback } from 'react';
import { DataStatus } from '../store/dataStatus';
import { ThemeName } from '../themeName';
import Weather from '../weather';
import styles from './styles.scss';

export interface AppProps {
  themeName: ThemeName;
  dataStatus: DataStatus;
  setTheme: (value: ThemeName) => void;
}

export const App: FunctionComponent<AppProps> = ({
  themeName,
  setTheme,
  dataStatus,
}) => {
  const toggleTheme = useCallback(() => {
    setTheme(themeName === 'dark' ? 'light' : 'dark');
  }, [themeName, setTheme]);

  return (
    <div className={styles.container}>
      <main
        className={classNames({
          [Classes.DARK]: themeName == 'dark',
        })}
      >
        <Navbar>
          <NavbarGroup>
            <Button
              minimal
              onClick={toggleTheme}
              text={`Current theme: {${themeName}} - Toggle?`}
            />
          </NavbarGroup>
          <NavbarGroup align={Alignment.RIGHT}>
            Data Status: {DataStatus[dataStatus].toUpperCase()}
          </NavbarGroup>
        </Navbar>

        <Weather />
      </main>
    </div>
  );
};
