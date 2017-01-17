using UnityEngine;
using System.Collections;

public interface ObjectLogger {

	void asyncLogger(AsyncEvent e);

    SyncEvent syncLogger(EventCode.GlobalUpdateEventCode code);

    void registerLoggerToEventLogger();
}
