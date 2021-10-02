package main

import (
	"context"
	"fmt"

	"github.com/gin-gonic/gin"
	"github.com/go-redis/redis/v8"
)

var ctx = context.Background()

func main() {
	StartAPI()
}

func StartAPI() {

	r := gin.Default()
	rdb := redis.NewClient(&redis.Options{
		Addr:     "localhost:6379",
		Password: "", // no password set
		DB:       0,  // use default DB
	})

	r.GET("/list/:name/add/:value", func(c *gin.Context) {
		listName := c.Param("name")
		key_value := c.Param("value")
		val, err := rdb.LPush(ctx, listName, key_value).Result()
		if err != nil {
			c.String(400, err.Error())
			c.Done()
		}

		c.String(200, fmt.Sprint(val))
	})

	// err := rdb.Set(ctx, "key", "value", 0).Err()
	// if err != nil {
	// 	panic(err)
	// }

	// val, err := rdb.Get(ctx, "key").Result()
	// if err != nil {
	// 	panic(err)
	// }

	// fmt.Println("key", val)

	// val2, err := rdb.Get(ctx, "key2").Result()
	// if err == redis.Nil {
	// 	fmt.Println("key2 does not exist")
	// } else if err != nil {
	// 	panic(err)
	// } else {
	// 	fmt.Println("key2", val2)
	// }

	// Output: key value
	// key2 does not exist

	r.Run(":9090")
}
