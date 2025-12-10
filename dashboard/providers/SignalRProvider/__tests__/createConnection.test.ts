// __tests__/createConnection.test.ts
import { createConnection } from '../createConnection';
import * as signalR from '@microsoft/signalr';

jest.mock('@microsoft/signalr');

describe('createConnection', () => {
  let buildMock: jest.Mock;
  let withUrlMock: jest.Mock;
  let configureLoggingMock: jest.Mock;
  let withAutomaticReconnectMock: jest.Mock;

  beforeEach(() => {
    buildMock = jest.fn().mockReturnValue('hub-connection-mock');
    withUrlMock = jest.fn().mockReturnThis();
    configureLoggingMock = jest.fn().mockReturnThis();
    withAutomaticReconnectMock = jest.fn().mockReturnThis();
    const builderMock = {
      withUrl: withUrlMock,
      configureLogging: configureLoggingMock,
      withAutomaticReconnect: withAutomaticReconnectMock,
      build: buildMock,
    };

    // @ts-expect-error(2322) - Mocking HubConnectionBuilder
    signalR.HubConnectionBuilder.mockImplementation(() => builderMock);
  });

  afterEach(() => {
    jest.resetAllMocks();
  });

  it('should create a HubConnection without autoReconnect', () => {
    const url = 'test-url';
    const result = createConnection({ url });
    
    expect(result).toBe('hub-connection-mock');
    expect(buildMock).toHaveBeenCalled();
    expect(withUrlMock).toHaveBeenCalledWith(url);
    expect(configureLoggingMock).toHaveBeenCalledWith(signalR.LogLevel.Information);
  });

  it('should create a HubConnection with autoReconnect', () => {
    const url = 'test-url';
    const result = createConnection({ url, autoReconnect: true });

    expect(result).toBe('hub-connection-mock');
    expect(buildMock).toHaveBeenCalled();
    expect(withUrlMock).toHaveBeenCalledWith(url);
    expect(configureLoggingMock).toHaveBeenCalledWith(signalR.LogLevel.Information);
    expect(withAutomaticReconnectMock).toHaveBeenCalled();
  });
});
