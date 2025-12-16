import { render, act, screen, renderHook } from "@testing-library/react";
import { SignalRContext, SignalRProvider } from "../HostSignalRProvider";
import { useContext } from "react";
import { createConnection } from "../createConnection";
import { HandlerFunction } from "@/types/common";

jest.mock('../createConnection');

const startMock = jest.fn().mockResolvedValue(undefined);
const stopMock = jest.fn().mockResolvedValue(undefined);
const onMock = jest.fn();
const offMock = jest.fn();
const invokeMock = jest.fn();
const stateMock = 'Connected';

beforeEach(() => {
  jest.clearAllMocks();
  (createConnection as jest.Mock).mockReturnValue({
    start: startMock,
    stop: stopMock,
    on: onMock,
    off: offMock,
    invoke: invokeMock,
    state: stateMock
  });
});

type TestConsumerProps = {
  on?: HandlerFunction;
  off?: HandlerFunction;
};
const TestConsumer = ({on = () => {}, off = () => {}}: TestConsumerProps) => {
  const context = useContext(SignalRContext);
  
  return (
    <div>
      <span data-testid="status">{context?.status}</span>
      <button
        onClick={() => context?.on("HostOnline", on)}
        data-testid="btn-on"
      />
      <button
        onClick={() => context?.off("HostOnline", off)}
        data-testid="btn-off"
      />
      <button
        onClick={() => context?.invoke("ping")}
        data-testid="btn-invoke"
      />
    </div>
  );
}

const Wrapper = ({ children }: { children: React.ReactNode }) => 
  <SignalRProvider url="/test">{children}</SignalRProvider>;

describe('SignalRProvider', () => {
  it('should create and start the connection on mount', async () => {
    await act(async () => {
      render(
        <SignalRProvider url="/test-url" autoReconnect>
          <TestConsumer />
        </SignalRProvider>
      );
    });

    expect(createConnection).toHaveBeenCalledTimes(1);
    expect(createConnection).toHaveBeenCalledWith({
      url: "/test-url",
      autoReconnect: true,
    });
    expect(startMock).toHaveBeenCalledTimes(1);
  });

  it('should stop the connection on unmount', async () => {
    const { unmount } = render(
      <SignalRProvider url="/test-url">
        <TestConsumer />
      </SignalRProvider>
    );

    await act(async () => {
      unmount();
    });

    expect(stopMock).toHaveBeenCalledTimes(1);
  });

  it('should set status to connected after start succeeds', async () => {
    await act(async () => {
      render(
        <SignalRProvider url="/x">
          <TestConsumer />
        </SignalRProvider>
      );
    });

    expect(screen.getByTestId("status").textContent).toBe("connected");
  });

  it("should set status to available if start fails", async () => {
    startMock.mockRejectedValueOnce(new Error("fail"));

    await act(async () => {
      render(
        <SignalRProvider url="/x">
          <TestConsumer />
        </SignalRProvider>
      );
    });

    expect(screen.getByTestId("status").textContent).toBe("unavailable");
  });

  it("should call connection.on() when context.on() is invoked", async () => {

    const handler = jest.fn();

    render(
        <SignalRProvider url="/url">
          <TestConsumer on={handler}/>
        </SignalRProvider>
      );

      await act(async () => {
        screen.getByTestId("btn-on").click();
      });

      expect(onMock).toHaveBeenCalledWith("HostOnline", handler);
  });

  it("should call connection.off() when context.off() is invoked", async () => {
    const handler = jest.fn();
    render(
        <SignalRProvider url="/url">
          <TestConsumer on={ handler } off={ handler }/>
        </SignalRProvider>
      );

      await act(async () => {
        screen.getByTestId("btn-on").click();
        screen.getByTestId("btn-off").click();
      });

      expect(offMock).toHaveBeenCalledWith("HostOnline", handler);
  });

  it("should call connection.invoke() when context.invoke() is invoked", async () => {
    invokeMock.mockResolvedValueOnce("pong");

    render(
        <SignalRProvider url="/url">
          <TestConsumer />
        </SignalRProvider>
      );

      await act(async () => {
        screen.getByTestId("btn-invoke").click();
      });

      expect(invokeMock).toHaveBeenCalledWith("ping");
  });

  it("should allow multiple components to register event handlers without interfering with each other", async () => {
    
    const Wrapper = ({ children }: { children: React.ReactNode }) => (
      <SignalRProvider url="/test">
        {children}
      </SignalRProvider>
    );
    const { result: resultA } = renderHook(() => useContext(SignalRContext), { wrapper: Wrapper });
    const { result: resultB } = renderHook(() => useContext(SignalRContext), { wrapper: Wrapper });

    act(() => {
      resultA.current?.on("HostOnline", jest.fn());
      resultB.current?.on("HostOnline", jest.fn());
    });

    expect(onMock).toHaveBeenCalledTimes(2);
    expect(onMock).toHaveBeenNthCalledWith(1, "HostOnline", expect.any(Function));
    expect(onMock).toHaveBeenNthCalledWith(2, "HostOnline", expect.any(Function));
  });

  it("should only register the same handler once even if on is called multiple times", () => {
    const handler = jest.fn();

    const { result } = renderHook(() => useContext(SignalRContext), { wrapper: Wrapper });

    act(() => {
      result.current?.on("HostOnline", handler);
      result.current?.on("HostOnline", handler);
    });

    expect(onMock).toHaveBeenCalledTimes(1);
    expect(onMock).toHaveBeenCalledWith("HostOnline", handler);
  });

  it("should only remove the exact handler", () => {
    const handlerA = jest.fn();
    const handlerB = jest.fn();

    const { result } = renderHook(() => useContext(SignalRContext), { wrapper: Wrapper });

    act(() => {
      result.current?.on("HostOnline", handlerA);
      result.current?.on("HostOnline", handlerB);
      result.current?.off("HostOnline", handlerA);
    });

    expect(offMock).toHaveBeenCalledTimes(1);
    expect(offMock).toHaveBeenCalledWith("HostOnline", handlerA);
  });

  it("should register multiple different handlers without conflict", () => {
    const handlerA = jest.fn();
    const handlerB = jest.fn();

    const { result } = renderHook(() => useContext(SignalRContext), { wrapper: Wrapper });

    act(() => {
      result.current?.on("HostOnline", handlerA);
      result.current?.on("HostOnline", handlerB);
    });

    expect(onMock).toHaveBeenCalledTimes(2);
    expect(onMock).toHaveBeenCalledWith("HostOnline", handlerA);
    expect(onMock).toHaveBeenCalledWith("HostOnline", handlerB);
  });

  it("warns when off is called with a callback that was never registered", () => {
    const warnSpy = jest.spyOn(console, "warn").mockImplementation(() => {});

    const handler = jest.fn();

    const { result } = renderHook(() => useContext(SignalRContext), { wrapper: Wrapper });

    act(() => {
      result.current?.off("HostOnline", handler);
    });

    expect(warnSpy).toHaveBeenCalledWith(
      expect.stringContaining("callback not found")
    );

    warnSpy.mockRestore();
  });

});