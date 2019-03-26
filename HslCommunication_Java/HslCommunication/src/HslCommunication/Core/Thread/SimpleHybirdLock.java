package HslCommunication.Core.Thread;

import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;

/**
 * 系统的锁对象
 */
public class SimpleHybirdLock
{
    /**
     * 实例化一个新的锁的对象
     */
    public SimpleHybirdLock(){
        queueLock = new ReentrantLock();                // 实例化数据访问锁
    }

    /**
     * 获取锁
     */
    public void Enter()
    {
        queueLock.lock();
    }

    /**
     * 离开锁
     */
    public void Leave(){
        queueLock.unlock();
    }

    
    private Lock queueLock = null;                      // 数据访问的同步锁
}
