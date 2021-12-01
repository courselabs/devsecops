package co.courselabs.helloworld;

import java.io.File;
import java.security.SecureRandom;

public class HelloWorld {
    public static void main(String[] args) {
        File tempDir;
        try {
            tempDir = File.createTempFile("courselabs", ".");
            tempDir.delete();
            tempDir.mkdir();
        }
        catch (java.io.IOException ioex) {}

        SecureRandom sr = new SecureRandom();
        sr.setSeed(123456L);

        System.out.println("Hello, World");
    }
}