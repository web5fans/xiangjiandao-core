using NetCorePal.Extensions.Snowflake;

namespace Xiangjiandao.Web.Utils;

public class WarpSnowflakeIdGenerator : ISnowflakeIdGenerator
{
    public long NextId()
    {
        return SnowflakeIdGenerator.GetNextValue();
    }
}

public class XiangjiandaoSnowflakeIdGenerator : ISnowflakeIdGenerator
{
    // ==============================Fields===========================================
    /**
     * 开始时间截 (2020-01-01)
     */
    private const long Twepoch = 1577808000000L;

    /**
     * 机器id所占的位数
     */
    private const int WorkerIdBits = 5;

    /**
     * 数据标识id所占的位数
     */
    private const int DatacenterIdBits = 5;

    /**
     * 支持的最大机器id，结果是31 (这个移位算法可以很快的计算出几位二进制数所能表示的最大十进制数)
     */
    private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);

    /**
     * 支持的最大数据标识id，结果是31
     */
    private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

    /**
     * 序列在id中占的位数
     */
    private const int SequenceBits = 12;

    /**
     * 机器ID向左移12位
     */
    private const int WorkerIdShift = SequenceBits;

    /**
     * 数据标识id向左移17位(12+5)
     */
    private const int DatacenterIdShift = SequenceBits + WorkerIdBits;

    /**
     * 时间截向左移22位(5+5+12)
     */
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

    /**
     * 生成序列的掩码，这里为4095 (0b111111111111=0xfff=4095)
     */
    private const long SequenceMask = -1L ^ (-1L << SequenceBits);

    /**
     * 工作机器ID(0~31)
     */
    private readonly long _workerId;

    /**
     * 数据中心ID(0~31)
     */
    private readonly long _datacenterId;

    /**
     * 毫秒内序列(0~4095)
     */
    private long _sequence;

    /**
     * 上次生成ID的时间截
     */
    private long _lastTimestamp = -1L;

    private readonly object _lock = new();

    //==============================Constructors=====================================

    /**
     * 构造函数
     *
     * @param workerId     工作ID (0~31)
     * @param datacenterId 数据中心ID (0~31)
     */
    public XiangjiandaoSnowflakeIdGenerator(long workerId, long datacenterId)
    {
        if (workerId > MaxWorkerId || workerId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(workerId),
                $"worker Id can't be greater than {MaxWorkerId} or less than 0");
        }

        if (datacenterId > MaxDatacenterId || datacenterId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(datacenterId),
                $"datacenter Id can't be greater than {MaxDatacenterId} or less than 0");
        }

        this._workerId = workerId;
        this._datacenterId = datacenterId;
    }

    // ==============================Methods==========================================

    /**
     * 获得下一个ID (该方法是线程安全的)
     *
     * @return SnowflakeId
     */
    public long NextId()
    {
        lock (_lock)
        {
            long timestamp = TimeGen();

            //如果当前时间小于上一次ID生成的时间戳，说明系统时钟回退过这个时候应当抛出异常
            if (timestamp < _lastTimestamp)
            {
                throw new ClockMovedBackwardsException(
                    $"Clock moved backwards.  Refusing to generate autoconfigure for {_lastTimestamp - timestamp} milliseconds");
            }

            //如果是同一时间生成的，则进行毫秒内序列
            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                //毫秒内序列溢出
                if (_sequence == 0)
                {
                    //阻塞到下一个毫秒,获得新的时间戳
                    timestamp = TilNextMillis(_lastTimestamp);
                }
            }
            //时间戳改变，毫秒内序列重置
            else
            {
                _sequence = 0L;
            }

            //上次生成ID的时间截
            _lastTimestamp = timestamp;

            //移位并通过或运算拼到一起组成64位的ID
            return ((timestamp - Twepoch) << TimestampLeftShift)
                   | (_datacenterId << DatacenterIdShift)
                   | (_workerId << WorkerIdShift)
                   | _sequence;
        }
    }

    /**
     * 阻塞到下一个毫秒，直到获得新的时间戳
     *
     * @param lastTimestamp 上次生成ID的时间截
     * @return 当前时间戳
     */
    long TilNextMillis(long lastTimestamp)
    {
        long timestamp = TimeGen();
        while (timestamp <= lastTimestamp)
        {
            timestamp = TimeGen();
        }

        return timestamp;
    }

    /**
     * 返回以毫秒为单位的当前时间
     *
     * @return 当前时间(毫秒)
     */
    private long TimeGen()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

#pragma warning disable S3925
    public sealed class ClockMovedBackwardsException(string message) : Exception(message);
#pragma warning restore S3925
}