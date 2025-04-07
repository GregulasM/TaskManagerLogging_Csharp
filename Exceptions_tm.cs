namespace TaskManager;


// ------ Отдельные классы для своих ошибок ------ // 

// NullOrWhiteSpace чисто для пустоты при выборе действия (>_<)
// TaskLenghtMinimal для длины таска меньше двух символов (^_^)
public class NullOrWhiteSpace : Exception
{
    public NullOrWhiteSpace(string message) : base(message){}
    
}

public class TaskLenghtMinimal : Exception
{
    public TaskLenghtMinimal(string message) : base(message){}
    
}

// ------------------------------------------------ // 